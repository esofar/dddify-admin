namespace Dddify.Admin.Application.Exceptions.Permissions;

public class PermissionNotFoundException : AppException
{
    public PermissionNotFoundException(Guid id)
    {
        WithErrorCode("permission_not_found");
        WithMetadata("PermissionId", id);
    }

    public PermissionNotFoundException(string code)
    {
        WithErrorCode("permission_not_found");
        WithMetadata("Code", code);
    }
}
