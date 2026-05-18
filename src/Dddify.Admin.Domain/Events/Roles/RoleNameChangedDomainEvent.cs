namespace Dddify.Admin.Domain.Events.Roles;

public record RoleNameChangedDomainEvent(Guid RoleId, string NewRoleName) : IDomainEvent;