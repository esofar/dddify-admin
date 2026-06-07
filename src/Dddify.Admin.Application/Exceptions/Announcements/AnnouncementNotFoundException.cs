namespace Dddify.Admin.Application.Exceptions.Announcements;

public class AnnouncementNotFoundException : AppException
{
    public AnnouncementNotFoundException(Guid announcementId)
    {
        WithErrorCode("announcement_not_found");
        WithMetadata("AnnouncementId", announcementId);
    }
}
