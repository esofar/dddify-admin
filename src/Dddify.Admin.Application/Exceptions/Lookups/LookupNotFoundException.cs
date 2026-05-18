namespace Dddify.Admin.Application.Exceptions.Lookups;

public class LookupNotFoundException : AppException
{
    public LookupNotFoundException(string code)
    {
        WithErrorCode("lookup_not_found");
        WithMetadata("Code", code);
    }

    public LookupNotFoundException(Guid id)
    {
        WithErrorCode("lookup_not_found");
        WithMetadata("LookupId", id);
    }
}
