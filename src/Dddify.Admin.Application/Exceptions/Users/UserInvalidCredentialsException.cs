namespace Dddify.Admin.Application.Exceptions.Users;

public class UserInvalidCredentialsException : AppException
{
    public UserInvalidCredentialsException(string account)
    {
        WithErrorCode("user_invalid_credentials");
        WithMetadata("Account", account);
    }
}
