using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.Events.Users;

public class UserEnabledDomainEventHandler(IEmailSender emailSender) : IDomainEventHandler<UserEnabledDomainEvent>
{
    public async Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken)
    {
        var subject = "账号已启用通知";
        var body = $"您好，您的账号已被管理员启用，可以正常登录系统。";
        await emailSender.SendAsync(@event.Email, subject, body);
    }
}
