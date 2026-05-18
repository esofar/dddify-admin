namespace Dddify.Admin.Domain.Aggregates.Roles;

public class RolePermission : Entity
{
    public const int MaxPermissionCodeLength = 50;

    public Guid RoleId { get; private set; }

    public Guid PermissionId { get; private set; }

    public string PermissionCode { get; private set; } = default!;

    private RolePermission() { }

    public RolePermission(Guid roleId, Guid permissionId, string permissionCode)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        PermissionCode = permissionCode;
    }

    public override object[] GetKeys()
    {
        return [RoleId, PermissionId];
    }
}
