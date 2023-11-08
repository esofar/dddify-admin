namespace Dddify.Admin.Domain.Exceptions.Users;

public class UserNotFoundException : DomainException
{
    public override string Name => "user_not_found";

    public UserNotFoundException(Guid? id)
        : base($"User with id '{id}' was not found.")
    {
    }
}