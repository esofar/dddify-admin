using Dddify.Admin.Application.Commands.Sessions;
using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.EventHandlers.Users;

public class UserDisabledDomainEventHandler(
    IEmailSender emailSender,
    ISender sender) : IDomainEventHandler<UserDisabledDomainEvent>
{
    public async Task Handle(UserDisabledDomainEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(
            new RevokeUserSessionsCommand(@event.UserId, "user_disabled"),
            cancellationToken);

        var subject = "账号已禁用通知";
        var body = $"您好，您的账号已被管理员禁用，如有疑问请联系管理员。";
        await emailSender.SendAsync(@event.Email, subject, body);
    }
}
