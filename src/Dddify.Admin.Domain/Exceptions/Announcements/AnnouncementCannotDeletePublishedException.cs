namespace Dddify.Admin.Domain.Exceptions.Announcements;

public class AnnouncementCannotDeletePublishedException : DomainException
{
    public AnnouncementCannotDeletePublishedException(Guid announcementId)
    {
        WithErrorCode("announcement_cannot_delete_published");
        WithMetadata("AnnouncementId", announcementId);
    }
}
