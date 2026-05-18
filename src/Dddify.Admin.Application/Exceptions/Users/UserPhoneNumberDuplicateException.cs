namespace Dddify.Admin.Application.Exceptions.Users;

public class UserPhoneNumberDuplicateException : AppException
{
    public UserPhoneNumberDuplicateException(string phoneNumber)
    {
        WithErrorCode("user_phone_number_duplicate");
        WithMetadata("PhoneNumber", phoneNumber);
    }
}
