using Dddify.Admin.Application.Dtos.InboxItems;

namespace Dddify.Admin.Application.Queries.InboxItems;

public record GetUnreadInboxItemCountQuery(Guid UserId) : IQuery<UnreadInboxItemCountDto>;

public class GetUnreadInboxItemCountQueryHandler(IInboxItemRepository inboxItemRepository)
    : IQueryHandler<GetUnreadInboxItemCountQuery, UnreadInboxItemCountDto>
{
    public async Task<UnreadInboxItemCountDto> Handle(GetUnreadInboxItemCountQuery query, CancellationToken cancellationToken)
    {
        var count = await inboxItemRepository.CountUserUnreadInboxItemsAsync(query.UserId, cancellationToken);

        return new UnreadInboxItemCountDto(count);
    }
}
