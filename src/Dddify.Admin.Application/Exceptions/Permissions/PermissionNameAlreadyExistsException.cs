namespace Dddify.Admin.Application.Exceptions.Permissions;

public class PermissionNameAlreadyExistsException : AppException
{
    public PermissionNameAlreadyExistsException(string name)
    {
        WithErrorCode("permission_name_already_exists");
        WithMetadata("Name", name);
    }
}
