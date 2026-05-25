using Dddify.Admin.Application.Commands.Sessions;
using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.Events.Users;

public class RevokeSessionsUserDisabledDomainEventHandler(
    ISender sender) : IDomainEventHandler<UserDisabledDomainEvent>
{
    public async Task Handle(UserDisabledDomainEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(
            new RevokeUserSessionsCommand(@event.UserId, "user_disabled"),
            cancellationToken);
    }
}
