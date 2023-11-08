namespace Dddify.Admin.Domain.Exceptions.Users;

public class UserEmailDuplicateException : DomainException
{
    public override string Name => "user_email_duplicate";

    public UserEmailDuplicateException(string email)
        : base($"User with email '{email}' already exists.")
    {
    }
}