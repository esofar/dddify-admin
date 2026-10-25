using Dddify.Admin.Application.Services.Email;

namespace Dddify.Admin.Infrastructure.Email;

public class FakeEmailSender(ILogger<FakeEmailSender> logger) : IEmailSender
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        logger.LogInformation("FAKE EMAIL SENT: To={To}, Subject={Subject}, Body={Body}", message.To, message.Subject, message.Body);

        return Task.CompletedTask;
    }
}
