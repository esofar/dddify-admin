using Dddify.Admin.Application.Services.Email;

namespace Dddify.Admin.Application.Services;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
