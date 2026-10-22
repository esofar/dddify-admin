using Dddify.Admin.Application.Exceptions.Email;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Sockets;

namespace Dddify.Admin.Infrastructure.Email;

public sealed class SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        string? from = null,
        IEnumerable<string>? cc = null,
        IEnumerable<string>? bcc = null)
    {
        var settings = options.Value;
        using var message = new MimeMessage
        {
            Subject = subject,
            Body = new TextPart(isHtml ? "html" : "plain") { Text = body }
        };
        var sender = MailboxAddress.Parse(from ?? settings.From);
        sender.Name = settings.DisplayName;
        message.From.Add(sender);
        message.To.Add(MailboxAddress.Parse(to));
        foreach (var address in cc ?? []) message.Cc.Add(MailboxAddress.Parse(address));
        foreach (var address in bcc ?? []) message.Bcc.Add(MailboxAddress.Parse(address));

        using var client = new SmtpClient();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var security = !settings.EnableSsl ? SecureSocketOptions.None
            : settings.Port == 465 ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

        try
        {
            await client.ConnectAsync(settings.Host, settings.Port, security, timeout.Token);
            if (!string.IsNullOrWhiteSpace(settings.UserName))
            {
                await client.AuthenticateAsync(settings.UserName, settings.Password, timeout.Token);
            }
            await client.SendAsync(message, timeout.Token);
        }
        catch (Exception exception) when (exception is SmtpCommandException or SmtpProtocolException
            or AuthenticationException or SslHandshakeException or ServiceNotAuthenticatedException
            or IOException or SocketException or OperationCanceledException or NotSupportedException)
        {
            logger.LogWarning("Email delivery failed: {ExceptionType}", exception.GetType().Name);
            throw new EmailSendFailedException();
        }

        try
        {
            await client.DisconnectAsync(true, timeout.Token);
        }
        catch (Exception exception) when (exception is SmtpCommandException or SmtpProtocolException
            or IOException or SocketException or OperationCanceledException)
        {
            // The server already accepted the message; a failed QUIT must not trigger a resend.
            logger.LogWarning("SMTP disconnect failed after delivery: {ExceptionType}", exception.GetType().Name);
        }
    }
}
