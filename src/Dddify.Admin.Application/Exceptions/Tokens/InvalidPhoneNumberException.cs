namespace Dddify.Admin.Application.Exceptions.Tokens;

public class InvalidPhoneNumberException : AppException
{
    public override string Name => "invalid_phone_number";

    public InvalidPhoneNumberException(string phoneNumber)
        : base($"User with phone number '{phoneNumber}' does not exist.")
    {
    }
}