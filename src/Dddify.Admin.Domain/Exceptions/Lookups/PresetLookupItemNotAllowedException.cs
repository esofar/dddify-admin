namespace Dddify.Admin.Domain.Exceptions.Lookups;

public class LookupPresetItemNotAllowedException : DomainException
{
    public LookupPresetItemNotAllowedException(Guid lookupId, Guid itemId)
    {
        WithErrorCode("lookup_preset_item_not_allowed");
        WithMetadata(
        [
            new("LookupId", lookupId),
            new("ItemId", itemId),
        ]);
    }
}
