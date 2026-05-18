namespace Dddify.Admin.Domain.Events.Departments;

public sealed record DepartmentEnabledDomainEvent(Guid DepartmentId) : IDomainEvent;