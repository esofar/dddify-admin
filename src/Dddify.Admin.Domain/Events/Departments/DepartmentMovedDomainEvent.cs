namespace Dddify.Admin.Domain.Events.Departments;

public sealed record DepartmentMovedDomainEvent(
    Guid DepartmentId,
    string OldPath,
    string NewPath,
    string OldFullName,
    string NewFullName
) : IDomainEvent;