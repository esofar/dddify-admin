namespace Dddify.Admin.Application.Exceptions.Users;

public class UserEmailDuplicateException : AppException
{
    public UserEmailDuplicateException(string email)
    {
        WithErrorCode("user_email_duplicate");
        WithMetadata("Email", email);
    }
}
