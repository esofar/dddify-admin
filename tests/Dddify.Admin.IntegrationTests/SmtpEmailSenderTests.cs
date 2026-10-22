using Dddify.Admin.Application.Exceptions.Email;
using Dddify.Admin.Infrastructure.Email;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Dddify.Admin.IntegrationTests;

public sealed class SmtpEmailSenderTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Sends_Unicode_Body_And_All_Recipients_Without_Exposing_Bcc(bool isHtml)
    {
        await using var server = new SmtpTestServer();
        var settings = Settings(server);
        settings.UserName = "smtp-user";
        settings.Password = "smtp-password";
        var body = isHtml ? "<p>验证码：012345</p>" : "验证码：012345";

        await Sender(settings).SendAsync("to@example.com", "登录验证码", body, isHtml,
            "override@example.com", ["cc@example.com"], ["bcc@example.com"]);
        await server.Completion;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(server.Message));
        using var message = await MimeMessage.LoadAsync(stream);
        Assert.Equal("登录验证码", message.Subject);
        Assert.Equal(body, (isHtml ? message.HtmlBody : message.TextBody)?.TrimEnd('\r', '\n'));
        Assert.Equal("override@example.com", message.From.Mailboxes.Single().Address);
        Assert.Equal("Dddify Admin", message.From.Mailboxes.Single().Name);
        Assert.Equal("to@example.com", message.To.Mailboxes.Single().Address);
        Assert.Equal("cc@example.com", message.Cc.Mailboxes.Single().Address);
        Assert.Empty(message.Bcc);
        Assert.Equal(3, server.Recipients.Count);
        Assert.Contains(server.Recipients, recipient => recipient.Contains("bcc@example.com"));
        Assert.Equal("\0smtp-user\0smtp-password", server.Authentication);
    }

    [Fact]
    public async Task Maps_Server_Rejection_To_EmailSendFailedException()
    {
        await using var server = new SmtpTestServer(rejectMessage: true);
        await Assert.ThrowsAsync<EmailSendFailedException>(() =>
            Sender(Settings(server)).SendAsync("to@example.com", "Subject", "Body"));
        await server.Completion;
    }

    [Fact]
    public async Task Requires_StartTls_When_Encryption_Is_Enabled()
    {
        await using var server = new SmtpTestServer();
        var settings = Settings(server);
        settings.EnableSsl = true;
        await Assert.ThrowsAsync<EmailSendFailedException>(() =>
            Sender(settings).SendAsync("to@example.com", "Subject", "Body"));
        await server.Completion;
        Assert.Empty(server.Recipients);
        Assert.Empty(server.Message);
    }

    [Fact]
    public async Task Quit_Failure_Does_Not_Fail_An_Accepted_Message()
    {
        await using var server = new SmtpTestServer(rejectQuit: true);
        await Sender(Settings(server)).SendAsync("to@example.com", "Subject", "Body");
        await server.Completion;
        Assert.NotEmpty(server.Message);
    }

    private static EmailOptions Settings(SmtpTestServer server) => new()
    {
        Host = "127.0.0.1",
        Port = server.Port,
        EnableSsl = false,
        From = "sender@example.com"
    };

    private static SmtpEmailSender Sender(EmailOptions settings) =>
        new(Options.Create(settings), NullLogger<SmtpEmailSender>.Instance);

    // A loopback SMTP server exercises MailKit's actual wire protocol without external delivery.
    private sealed class SmtpTestServer : IAsyncDisposable
    {
        private readonly TcpListener listener = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource timeout = new(TimeSpan.FromSeconds(15));

        public int Port { get; }
        public Task Completion { get; }
        public string Message { get; private set; } = string.Empty;
        public string? Authentication { get; private set; }
        public List<string> Recipients { get; } = [];

        public SmtpTestServer(bool rejectMessage = false, bool rejectQuit = false)
        {
            listener.Start();
            Port = ((IPEndPoint)listener.LocalEndpoint).Port;
            Completion = RunAsync(rejectMessage, rejectQuit);
        }

        private async Task RunAsync(bool rejectMessage, bool rejectQuit)
        {
            using var client = await listener.AcceptTcpClientAsync(timeout.Token);
            await using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            await using var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true)
            {
                NewLine = "\r\n",
                AutoFlush = true
            };
            await writer.WriteLineAsync("220 localhost ESMTP ready");
            while (await reader.ReadLineAsync(timeout.Token) is { } line)
            {
                if (line.StartsWith("EHLO "))
                    await writer.WriteLineAsync("250-localhost\r\n250 AUTH PLAIN");
                else if (line.StartsWith("AUTH PLAIN "))
                {
                    Authentication = Encoding.UTF8.GetString(Convert.FromBase64String(line[11..]));
                    await writer.WriteLineAsync("235 Authenticated");
                }
                else if (line.StartsWith("RCPT TO:"))
                {
                    Recipients.Add(line);
                    await writer.WriteLineAsync("250 Recipient accepted");
                }
                else if (line == "DATA")
                {
                    await writer.WriteLineAsync("354 End with a dot");
                    var data = new StringBuilder();
                    while (await reader.ReadLineAsync(timeout.Token) is { } part && part != ".")
                        data.Append(part.StartsWith("..") ? part[1..] : part).Append("\r\n");
                    Message = data.ToString();
                    await writer.WriteLineAsync(rejectMessage ? "550 Message rejected" : "250 Message accepted");
                }
                else if (line == "QUIT")
                {
                    await writer.WriteLineAsync(rejectQuit ? "500 QUIT failed" : "221 Goodbye");
                    return;
                }
                else
                    await writer.WriteLineAsync("250 OK");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await timeout.CancelAsync();
            listener.Stop();
            try { await Completion; }
            catch (OperationCanceledException) when (timeout.IsCancellationRequested) { }
            finally { timeout.Dispose(); }
        }
    }
}
