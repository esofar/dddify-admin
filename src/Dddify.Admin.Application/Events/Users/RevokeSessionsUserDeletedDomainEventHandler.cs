using Dddify.Admin.Application.Commands.Sessions;
using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.Events.Users;

public class RevokeSessionsUserDeletedDomainEventHandler(
    ISender sender) : IDomainEventHandler<UserDeletedDomainEvent>
{
    public async Task Handle(UserDeletedDomainEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(
            new RevokeUserSessionsCommand(@event.UserId, "user_deleted"),
            cancellationToken);
    }
}
