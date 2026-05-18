namespace Dddify.Admin.Domain.Exceptions.Users;

public class UserRoleNotAssignedException : DomainException
{
    public UserRoleNotAssignedException(Guid userId, Guid roleId)
    {
        WithErrorCode("user_role_not_assigned");
        WithMetadata(
        [
            new("UserId", userId),
            new("RoleId", roleId),
        ]);
    }
}
