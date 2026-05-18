namespace Dddify.Admin.Application.Exceptions.Permissions;

public class PermissionCodeAlreadyExistsException : AppException
{
    public PermissionCodeAlreadyExistsException(string code)
    {
        WithErrorCode("permission_code_already_exists");
        WithMetadata("Code", code);
    }
}
