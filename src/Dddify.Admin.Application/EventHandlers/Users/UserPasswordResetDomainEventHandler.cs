using Dddify.Admin.Application.Commands.Sessions;
using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.EventHandlers.Users;

public class UserPasswordResetDomainEventHandler(
    IEmailSender emailSender,
    ISender sender) : IDomainEventHandler<UserPasswordResetDomainEvent>
{
    public async Task Handle(UserPasswordResetDomainEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(
            new RevokeUserSessionsCommand(@event.UserId, "password_reset"),
            cancellationToken);

        var subject = "密码重置通知";
        var body = $"您好，您的新密码为：{@event.NewPassword}，请及时登录并修改密码。";
        await emailSender.SendAsync(@event.Email, subject, body);
    }
}
