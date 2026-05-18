namespace Dddify.Admin.Domain.Events.Departments;

public record DepartmentRenamedDomainEvent(Guid DepartmentId, string DepartmentName) : IDomainEvent;