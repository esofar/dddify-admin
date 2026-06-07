namespace Dddify.Admin.Domain.Exceptions.InboxItems;

public class InboxItemDomainException : DomainException
{
    public InboxItemDomainException(string errorCode)
    {
        WithErrorCode(errorCode);
    }
}
