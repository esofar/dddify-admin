namespace Dddify.Admin.Domain.Exceptions.Lookups;

public class LookupItemCannotDeletedException : DomainException
{
    public LookupItemCannotDeletedException(Guid lookupId, Guid lookupItemId)
    {
        WithErrorCode("lookup_item_cannot_deleted");
        WithMetadata(
        [
            new("LookupId", lookupId),
            new("LookupItemId", lookupItemId),
        ]);
    }
}
