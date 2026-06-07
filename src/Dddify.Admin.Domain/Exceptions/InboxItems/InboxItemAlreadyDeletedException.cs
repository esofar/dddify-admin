namespace Dddify.Admin.Domain.Exceptions.InboxItems;

public class InboxItemAlreadyDeletedException : DomainException
{
    public InboxItemAlreadyDeletedException(Guid inboxItemId)
    {
        WithErrorCode("inbox_item_already_deleted");
        WithMetadata("InboxItemId", inboxItemId);
    }
}
