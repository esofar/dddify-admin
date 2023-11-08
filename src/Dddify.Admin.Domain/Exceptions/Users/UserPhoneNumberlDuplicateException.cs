namespace Dddify.Admin.Domain.Exceptions.Users;

public class UserPhoneNumberDuplicateException : DomainException
{
    public override string Name => "user_phone_number_duplicate";

    public UserPhoneNumberDuplicateException(string phoneNumber)
        : base($"User with phone number '{phoneNumber}' already exists.")
    {
    }
}