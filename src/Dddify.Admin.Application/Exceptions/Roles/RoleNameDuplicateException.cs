namespace Dddify.Admin.Application.Exceptions.Roles;

public class RoleNameDuplicateException : AppException
{
    public RoleNameDuplicateException(string name)
    {
        WithErrorCode("role_name_duplicate");
        WithMetadata("Name", name);
    }
}
