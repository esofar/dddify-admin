using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Domain.Exceptions.Announcements;

public class AnnouncementStatusInvalidException : DomainException
{
    public AnnouncementStatusInvalidException(Guid announcementId, AnnouncementStatus actualStatus, AnnouncementStatus expectedStatus)
    {
        WithErrorCode("announcement_status_invalid");
        WithMetadata("AnnouncementId", announcementId);
        WithMetadata("ActualStatus", actualStatus.ToString());
        WithMetadata("ExpectedStatus", expectedStatus.ToString());
    }
}
