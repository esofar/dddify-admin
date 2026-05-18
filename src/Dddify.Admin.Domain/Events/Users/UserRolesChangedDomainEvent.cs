namespace Dddify.Admin.Domain.Events.Users;

public sealed record UserRolesChangedDomainEvent(
    Guid UserId,
    IReadOnlyCollection<Guid> AddedRoleIds,
    IReadOnlyCollection<Guid> RemovedRoleIds
) : IDomainEvent;