namespace Dddify.Admin.Domain.Exceptions.Lookups;

public class LookupItemAlreadyDisabledException : DomainException
{
    public LookupItemAlreadyDisabledException(Guid lookupId, Guid itemId)
    {
        WithErrorCode("lookup_item_already_disabled");
        WithMetadata(
        [
            new("LookupId", lookupId),
            new("ItemId", itemId),
        ]);
    }
}
