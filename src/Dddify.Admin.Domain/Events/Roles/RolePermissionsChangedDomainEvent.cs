namespace Dddify.Admin.Domain.Events.Roles;

public sealed record RolePermissionsChangedDomainEvent(Guid RoleId) : IDomainEvent;
