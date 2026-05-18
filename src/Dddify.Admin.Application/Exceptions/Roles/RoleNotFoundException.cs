namespace Dddify.Admin.Application.Exceptions.Roles;

public class RoleNotFoundException : AppException
{
    public RoleNotFoundException(Guid roleId)
    {
        WithErrorCode("role_not_found");
        WithMetadata("RoleId", roleId);
    }
}
