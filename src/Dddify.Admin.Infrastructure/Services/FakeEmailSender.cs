namespace Dddify.Admin.Infrastructure.Services;

[SingletonDependency(RegistrationMode.AsImplementedInterfaces)]
public class FakeEmailSender(ILogger<FakeEmailSender> logger) : IEmailSender
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
        logger.LogInformation("FAKE EMAIL SENT: To={To}, Subject={Subject}, Body={Body}", to, subject, body);

        await Task.CompletedTask;
    }
}
