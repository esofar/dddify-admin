namespace Dddify.Admin.Domain.Exceptions.Users;

public class UserAlreadyEnabledException : DomainException
{
    public UserAlreadyEnabledException(Guid userId)
    {
        WithErrorCode("user_already_enabled");
        WithMetadata("UserId", userId);
    }
}
