using Dddify.Admin.Application.Commands.InboxItems;
using Dddify.Admin.Application.Services.Announcements;
using Dddify.Admin.Domain.Aggregates.InboxItems;
using Dddify.Admin.Domain.Events.Announcements;

namespace Dddify.Admin.Application.Events.Announcements;

public class AnnouncementPublishedDomainEventHandler(
    IAudienceResolver audienceResolver,
    ISender sender) : IDomainEventHandler<AnnouncementPublishedDomainEvent>
{
    private const int BatchSize = 1000;

    public async Task Handle(AnnouncementPublishedDomainEvent @event, CancellationToken cancellationToken)
    {
        await foreach (var userIds in audienceResolver.ResolveUserIdsAsync(@event.Audience, BatchSize, cancellationToken))
        {
            await sender.Send(
                new CreateInboxItemsCommand(
                    userIds,
                    @event.Title,
                    @event.Summary,
                    InboxItemSourceType.Announcement,
                    @event.AnnouncementId),
                cancellationToken);
        }
    }
}
