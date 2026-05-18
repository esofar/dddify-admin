using Dddify.Admin.Application.Commands.Users;
using Dddify.Admin.Domain.Events.Departments;

namespace Dddify.Admin.Application.EventHandlers.Departments;

public class DepartmentRenamedDomainEventHandler(ISender sender) : IDomainEventHandler<DepartmentRenamedDomainEvent>
{
    public Task Handle(DepartmentRenamedDomainEvent @event, CancellationToken cancellationToken)
    {
        return sender.Send(new RefreshUsersDepartmentCommand(
            @event.DepartmentId,
            @event.DepartmentName), cancellationToken);
    }
}