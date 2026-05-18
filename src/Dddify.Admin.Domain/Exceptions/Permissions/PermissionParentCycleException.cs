namespace Dddify.Admin.Domain.Exceptions.Permissions;

public class PermissionParentCycleException : DomainException
{
    public PermissionParentCycleException(Guid permissionId, Guid parentId)
    {
        WithErrorCode("permission_parent_cycle");
        WithMetadata(
        [
            new("PermissionId", permissionId),
            new("ParentId", parentId),
        ]);
    }
}
