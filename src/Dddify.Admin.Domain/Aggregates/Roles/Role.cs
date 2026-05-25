using Dddify.Admin.Domain.Events.Roles;
using Dddify.Admin.Domain.Exceptions.Roles;

namespace Dddify.Admin.Domain.Aggregates.Roles;

public class Role : AuditableAggregateRoot<Guid>, IHasConcurrencyStamp
{
    public const int MaxNameLength = 50;
    public const int MaxDescriptionLength = 100;

    private readonly List<RolePermission> _permissions = [];

    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public bool IsPreset { get; private set; }

    public bool IsDefault { get; private set; }

    public int AssignedUserCount { get; set; }

    public int Order { get; private set; }

    public IReadOnlyList<RolePermission> Permissions => _permissions.AsReadOnly();

    public string? ConcurrencyStamp { get; set; }

    private Role() { }

    public Role(Guid id, string name, bool isDefault, int order, string? description)
    {
        Id = id;
        Name = name;
        IsDefault = isDefault;
        Order = order;
        Description = description;
    }

    public void Change(string name, bool isDefault, int order, string? description)
    {
        IsDefault = isDefault;
        Order = order;
        Description = description;

        if (Name != name)
        {
            Name = name;
            AddDomainEvent(new RoleNameChangedDomainEvent(Id, name));
        }
    }

    public void IncreaseAssignedUserCount()
    {
        AssignedUserCount++;
    }

    public void DecreaseAssignedUserCount()
    {
        if (AssignedUserCount > 0) AssignedUserCount--;
    }

    public void AssignPermissions(IEnumerable<RolePermission> permissions)
    {
        var targetPermissions = permissions.ToList();

        var comparer = EqualityComparer<RolePermission>.Create(
            (x, y) =>
                x != null && y != null &&
                x.PermissionId == y.PermissionId &&
                x.PermissionCode == y.PermissionCode,
            obj => obj is null ? 0 : HashCode.Combine(obj.PermissionId, obj.PermissionCode)
        );

        var currentPermissions = _permissions.ToList();
        var permissionsToRemove = currentPermissions.Except(targetPermissions, comparer);
        var permissionsToAdd = targetPermissions.Except(currentPermissions, comparer);

        var removed = permissionsToRemove.ToList();
        var added = permissionsToAdd.ToList();

        _permissions.RemoveAll(p => removed.Contains(p, comparer));
        _permissions.AddRange(added);

        if (removed.Count > 0 || added.Count > 0)
        {
            AddDomainEvent(new RolePermissionsChangedDomainEvent(Id));
        }
    }

    public void EnsureCanDelete()
    {
        if (IsPreset)
        {
            throw new PresetRoleCannotBeDeletedException(Id);
        }

        if (AssignedUserCount > 0)
        {
            throw new RoleHasAssignedUsersException(Id, AssignedUserCount);
        }
    }
}
