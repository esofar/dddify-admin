using Dddify.Admin.Application.Commands.Departments;
using Dddify.Admin.Domain.Events.Departments;

namespace Dddify.Admin.Application.EventHandlers.Departments;

public class DepartmentMovedDomainEventHandler(ISender sender) : IDomainEventHandler<DepartmentMovedDomainEvent>
{
    public Task Handle(DepartmentMovedDomainEvent @event, CancellationToken cancellationToken)
    {
        return sender.Send(new RefreshDepartmentHierarchyCommand(
            @event.DepartmentId,
            @event.OldPath,
            @event.NewPath,
            @event.OldFullName,
            @event.NewFullName), cancellationToken);
    }
}