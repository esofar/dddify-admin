using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.Events.Users;

public class SendUserDisabledEmailDomainEventHandler(
    IEmailSender emailSender) : IDomainEventHandler<UserDisabledDomainEvent>
{
    public async Task Handle(UserDisabledDomainEvent @event, CancellationToken cancellationToken)
    {
        var subject = "账号禁用通知";
        var body = "您好，您的账号已被管理员禁用，如有疑问请联系管理员。";

        await emailSender.SendAsync(@event.Email, subject, body);
    }
}
