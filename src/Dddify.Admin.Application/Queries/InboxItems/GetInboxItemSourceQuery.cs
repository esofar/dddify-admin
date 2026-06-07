using Dddify.Admin.Application.Dtos.Announcements;
using Dddify.Admin.Application.Dtos.InboxItems;
using Dddify.Admin.Application.Exceptions.InboxItems;
using Dddify.Admin.Domain.Aggregates.Announcements;
using Dddify.Admin.Domain.Aggregates.InboxItems;

namespace Dddify.Admin.Application.Queries.InboxItems;

public record GetInboxItemSourceQuery(Guid Id, Guid UserId) : IQuery<InboxItemSourceDetailDto>;

public class GetInboxItemSourceQueryHandler(
    IInboxItemRepository inboxItemRepository,
    IAnnouncementRepository announcementRepository) : IQueryHandler<GetInboxItemSourceQuery, InboxItemSourceDetailDto>
{
    public async Task<InboxItemSourceDetailDto> Handle(GetInboxItemSourceQuery query, CancellationToken cancellationToken)
    {
        var inboxItem = await inboxItemRepository
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.Id && c.UserId == query.UserId, cancellationToken)
            ?? throw new InboxItemNotFoundException(query.Id);

        var inboxItemSource = inboxItem.Source;

        if (inboxItemSource.Type != InboxItemSourceType.Announcement)
        {
            return new InboxItemSourceDetailDto(inboxItemSource.Type, inboxItemSource.Id, false, "unsupported_source_type", null);
        }

        var announcement = await announcementRepository
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == inboxItemSource.Id && c.Status == AnnouncementStatus.Published, cancellationToken);

        if (announcement is null)
        {
            return new InboxItemSourceDetailDto(inboxItemSource.Type, inboxItemSource.Id, false, "source_not_found", null);
        }

        var announcementDetail = announcement.Adapt<AnnouncementDetailDto>();

        return new InboxItemSourceDetailDto(inboxItemSource.Type, inboxItemSource.Id, true, null, announcementDetail);
    }
}
