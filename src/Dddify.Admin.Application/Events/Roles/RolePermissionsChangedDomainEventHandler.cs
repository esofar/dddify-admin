using Dddify.Admin.Application.Commands.Sessions;
using Dddify.Admin.Domain.Events.Roles;

namespace Dddify.Admin.Application.Events.Roles;

public class RolePermissionsChangedDomainEventHandler(ISender sender) : IDomainEventHandler<RolePermissionsChangedDomainEvent>
{
    public Task Handle(RolePermissionsChangedDomainEvent @event, CancellationToken cancellationToken)
    {
        return sender.Send(
            new RevokeRoleUsersSessionsCommand(@event.RoleId, "role_permissions_changed"),
            cancellationToken);
    }
}
