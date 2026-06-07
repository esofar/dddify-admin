namespace Dddify.Admin.Domain.Exceptions.Announcements;

public class AnnouncementDomainException : DomainException
{
    public AnnouncementDomainException(string errorCode)
    {
        WithErrorCode(errorCode);
    }
}
