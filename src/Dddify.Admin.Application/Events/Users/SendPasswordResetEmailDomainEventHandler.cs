using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.Events.Users;

public class SendPasswordResetEmailDomainEventHandler(
    IEmailSender emailSender) : IDomainEventHandler<UserPasswordResetDomainEvent>
{
    public async Task Handle(UserPasswordResetDomainEvent @event, CancellationToken cancellationToken)
    {
        var subject = "账号密码重置通知";
        var body = $"您好，管理员已重置您的账户密码，新密码为：{@event.NewPassword}。请使用新的登录凭据访问系统。";

        await emailSender.SendAsync(@event.Email, subject, body);
    }
}
