namespace Dddify.Admin.Domain.Events.Departments;

public record DepartmentLeaderChangedDomainEvent(
    Guid DepartmentId,
    Guid LeaderId,
    string LeaderName) : IDomainEvent;