namespace Dddify.Admin.Application.Exceptions.Users;

public class UserInvalidCredentialsException : AppException
{
    public UserInvalidCredentialsException(string email)
    {
        WithErrorCode("user_invalid_credentials");
        WithMetadata("Email", email);
    }
}
