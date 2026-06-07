using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Domain.Events.Announcements;

public record AnnouncementPublishedDomainEvent(
    Guid AnnouncementId,
    string Title,
    string Summary,
    AnnouncementAudience Audience) : IDomainEvent;
