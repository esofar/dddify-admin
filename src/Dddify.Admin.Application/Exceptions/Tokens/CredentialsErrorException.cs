namespace Dddify.Admin.Application.Exceptions.Tokens;

public class CredentialsErrorException : AppException
{
    public override string Name => "credentials_error";

    public CredentialsErrorException(string email)
        : base($"Credentials with email '{email}' or password is not correct.")
    {
    }
}