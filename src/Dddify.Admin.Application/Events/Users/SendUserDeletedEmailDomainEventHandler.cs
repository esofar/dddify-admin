using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.Events.Users;

public class SendUserDeletedEmailDomainEventHandler(
    IEmailSender emailSender) : IDomainEventHandler<UserDeletedDomainEvent>
{
    public async Task Handle(UserDeletedDomainEvent @event, CancellationToken cancellationToken)
    {
        var subject = "账户已删除通知";
        var body = "您好，管理员已删除您的账户，您的账号将无法继续登录或访问系统。为保障账户安全，系统已自动注销您所有已登录设备。如有疑问，请联系管理员。\n\n谢谢。";

        await emailSender.SendAsync(@event.Email, subject, body);
    }
}
