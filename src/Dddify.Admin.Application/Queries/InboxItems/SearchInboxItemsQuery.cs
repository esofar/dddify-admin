using Dddify.Admin.Application.Dtos.InboxItems;

namespace Dddify.Admin.Application.Queries.InboxItems;

public record SearchInboxItemsQuery(
    Guid UserId,
    int Current,
    int PageSize,
    bool? IsRead) : IQuery<IPagedResult<InboxItemListDto>>;

public class SearchInboxItemsQueryHandler(IInboxItemRepository inboxItemRepository) : IQueryHandler<SearchInboxItemsQuery, IPagedResult<InboxItemListDto>>
{
    public async Task<IPagedResult<InboxItemListDto>> Handle(SearchInboxItemsQuery query, CancellationToken cancellationToken)
    {
        return await inboxItemRepository
            .AsQueryable()
            .AsNoTracking()
            .Where(c => c.UserId == query.UserId)
            .WhereIf(query.IsRead.HasValue, c => c.IsRead == query.IsRead!.Value)
            .OrderByDescending(c => c.CreatedAt)
            .ProjectToType<InboxItemListDto>()
            .ToPagedResultAsync(query.Current, query.PageSize, cancellationToken);
    }
}
