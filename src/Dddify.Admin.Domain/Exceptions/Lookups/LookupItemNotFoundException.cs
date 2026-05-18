namespace Dddify.Admin.Domain.Exceptions.Lookups;

public class LookupItemNotFoundException : DomainException
{
    public LookupItemNotFoundException(Guid lookupId, Guid itemId)
    {
        WithErrorCode("lookup_item_not_found");
        WithMetadata(
        [
            new("LookupId", lookupId),
            new("ItemId", itemId),
        ]);
    }
}
