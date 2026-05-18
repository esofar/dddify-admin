using Dddify.Admin.Application.Commands.Roles;
using Dddify.Admin.Application.Commands.Sessions;
using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.EventHandlers.Users;

public class UserRolesChangedDomainEventHandler(ISender sender) : IDomainEventHandler<UserRolesChangedDomainEvent>
{
    public async Task Handle(UserRolesChangedDomainEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(new RefreshRoleUserCountCommand(
            @event.AddedRoleIds,
            @event.RemovedRoleIds), cancellationToken);

        await sender.Send(
            new RevokeUserSessionsCommand(@event.UserId, "user_roles_changed"),
            cancellationToken);
    }
}
