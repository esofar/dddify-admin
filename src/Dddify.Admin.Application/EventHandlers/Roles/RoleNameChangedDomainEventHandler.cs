using Dddify.Admin.Application.Commands.Users;
using Dddify.Admin.Domain.Events.Roles;

namespace Dddify.Admin.Application.EventHandlers.Roles;

public class RoleNameChangedDomainEventHandler(ISender sender) : IDomainEventHandler<RoleNameChangedDomainEvent>
{
    public async Task Handle(RoleNameChangedDomainEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(new RefreshUsersRoleCommand(@event.RoleId, @event.NewRoleName), cancellationToken);
    }
}