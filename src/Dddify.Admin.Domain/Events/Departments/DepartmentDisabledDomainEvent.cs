namespace Dddify.Admin.Domain.Events.Departments;

public sealed record DepartmentDisabledDomainEvent(Guid DepartmentId) : IDomainEvent;