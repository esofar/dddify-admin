using Dddify.Admin.Application.Commands.Roles;
using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.Events.Users;

public class UserRolesChangedDomainEventHandler(ISender sender) : IDomainEventHandler<UserRolesChangedDomainEvent>
{
    public async Task Handle(UserRolesChangedDomainEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(new RecalculateAssignedUserCountCommand(
            @event.AddedRoleIds,
            @event.RemovedRoleIds), cancellationToken);
    }
}
