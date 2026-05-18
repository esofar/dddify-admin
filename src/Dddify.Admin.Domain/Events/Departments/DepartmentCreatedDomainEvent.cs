namespace Dddify.Admin.Domain.Events.Departments;

public record DepartmentCreatedDomainEvent(Guid DepartmentId, Guid? ParentId, string Path) : IDomainEvent;