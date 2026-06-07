namespace Dddify.Admin.Domain.Events.Announcements;

public record AnnouncementWithdrawnDomainEvent(Guid AnnouncementId, DateTimeOffset WithdrawnAt) : IDomainEvent;
