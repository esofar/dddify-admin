namespace Dddify.Admin.Application.Services;

public interface IEmailSender
{
    Task SendAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        string? from = null,
        IEnumerable<string>? cc = null,
        IEnumerable<string>? bcc = null
    );
}
