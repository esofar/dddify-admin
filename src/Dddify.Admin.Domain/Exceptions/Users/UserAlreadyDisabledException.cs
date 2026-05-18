namespace Dddify.Admin.Domain.Exceptions.Users;

public class UserAlreadyDisabledException : DomainException
{
    public UserAlreadyDisabledException(Guid userId)
    {
        WithErrorCode("user_already_disabled");
        WithMetadata("UserId", userId);
    }
}
