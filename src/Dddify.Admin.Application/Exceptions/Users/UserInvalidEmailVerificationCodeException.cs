namespace Dddify.Admin.Application.Exceptions.Users;

public class UserInvalidEmailVerificationCodeException : AppException
{
    public UserInvalidEmailVerificationCodeException()
    {
        WithErrorCode("user_invalid_email_code");
    }
}
