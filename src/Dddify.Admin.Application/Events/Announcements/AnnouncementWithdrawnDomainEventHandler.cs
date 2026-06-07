using Dddify.Admin.Domain.Events.Announcements;

namespace Dddify.Admin.Application.Events.Announcements;

public class AnnouncementWithdrawnDomainEventHandler : IDomainEventHandler<AnnouncementWithdrawnDomainEvent>
{
    public Task Handle(AnnouncementWithdrawnDomainEvent @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
