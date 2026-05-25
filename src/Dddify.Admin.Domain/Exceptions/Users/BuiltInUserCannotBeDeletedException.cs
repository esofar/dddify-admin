namespace Dddify.Admin.Domain.Exceptions.Users;

public class BuiltInUserCannotBeDeletedException : DomainException
{
    public BuiltInUserCannotBeDeletedException(Guid id)
    {
        WithErrorCode("user_built_in_cannot_be_deleted");
        WithMetadata("UserId", id);
    }
}
