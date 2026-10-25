using Dddify.Admin.Application.Exceptions.Email;
using Dddify.Admin.Application.Abstractions.Email;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Sockets;

namespace Dddify.Admin.Infrastructure.Email;

public sealed class SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var settings = options.Value;
        using var message = new MimeMessage
        {
            Subject = email.Subject,
            Body = new TextPart(email.IsHtml ? "html" : "plain") { Text = email.Body }
        };
        var sender = MailboxAddress.Parse(email.From ?? settings.From);
        sender.Name = settings.DisplayName;
        message.From.Add(sender);
        message.To.Add(MailboxAddress.Parse(email.To));
        foreach (var address in email.Cc) message.Cc.Add(MailboxAddress.Parse(address));
        foreach (var address in email.Bcc) message.Bcc.Add(MailboxAddress.Parse(address));

        using var client = new SmtpClient();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
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
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
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
