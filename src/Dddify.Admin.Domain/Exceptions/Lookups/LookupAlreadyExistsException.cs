namespace Dddify.Admin.Domain.Exceptions.Lookups;

public class LookupAlreadyExistsException : DomainException
{
    public LookupAlreadyExistsException(string code, string name)
    {
        WithErrorCode("lookup_already_exists");
        WithMetadata(
        [
            new("Code", code),
            new("Name", name),
        ]);
    }
}
