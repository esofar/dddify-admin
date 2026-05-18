namespace Dddify.Admin.Application.Exceptions.Users;

public class UserNotFoundException : AppException
{
    public UserNotFoundException(Guid userId)
    {
        WithErrorCode("user_not_found");
        WithMetadata("UserId", userId);
    }
}
