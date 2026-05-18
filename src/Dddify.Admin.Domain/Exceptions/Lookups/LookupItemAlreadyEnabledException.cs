namespace Dddify.Admin.Domain.Exceptions.Lookups;

public class LookupItemAlreadyEnabledException : DomainException
{
    public LookupItemAlreadyEnabledException(Guid lookupId, Guid itemId)
    {
        WithErrorCode("lookup_item_already_enabled");
        WithMetadata(
        [
            new("LookupId", lookupId),
            new("ItemId", itemId),
        ]);
    }
}
