namespace Dddify.Admin.Domain.Exceptions.Lookups;

public class LookupHasItemsException : DomainException
{
    public LookupHasItemsException(Guid id)
    {
        WithErrorCode("lookup_has_items");
        WithMetadata("LookupId", id);
    }
}
