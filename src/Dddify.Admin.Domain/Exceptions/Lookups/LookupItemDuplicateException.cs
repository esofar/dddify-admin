namespace Dddify.Admin.Domain.Exceptions.Lookups;

public class LookupItemDuplicateException : DomainException
{
    public LookupItemDuplicateException(Guid lookupId, string? value, string? label)
    {
        WithErrorCode("lookup_item_duplicate");
        WithMetadata(
        [
            new("LookupId", lookupId),
            new("Value", value),
            new("Label", label),
        ]);
    }
}
