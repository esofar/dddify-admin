namespace Dddify.Admin.Application.Exceptions.Users;

public class UserInvalidSmsCodeException : AppException
{
    public UserInvalidSmsCodeException(string phoneNumber)
    {
        WithErrorCode("user_invalid_sms_code");
        WithMetadata("PhoneNumber", phoneNumber);
    }
}
