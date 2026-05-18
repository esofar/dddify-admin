namespace Dddify.Admin.Domain.Exceptions.Users;

public class UserRolesEmptyException : DomainException
{
    public UserRolesEmptyException(Guid userId)
    {
        WithErrorCode("user_roles_empty");
        WithMetadata("UserId", userId);
    }
}
