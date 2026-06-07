namespace Dddify.Admin.Application.Exceptions.InboxItems;

public class InboxItemNotFoundException : AppException
{
    public InboxItemNotFoundException(Guid inboxItemId)
    {
        WithErrorCode("inbox_item_not_found");
        WithMetadata("InboxItemId", inboxItemId);
    }
}
