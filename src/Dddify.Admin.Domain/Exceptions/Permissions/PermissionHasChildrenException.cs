namespace Dddify.Admin.Domain.Exceptions.Permissions;

public class PermissionHasChildrenException : DomainException
{
    public PermissionHasChildrenException(Guid permissionId)
    {
        WithErrorCode("permission_has_children");
        WithMetadata("PermissionId", permissionId);
    }
}
