using Dddify.Admin.Application.Dtos.Announcements;
using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Application.Queries.Announcements;

public record SearchAnnouncementsQuery(
    int Current,
    int PageSize,
    string? Keyword,
    string? Status) : IQuery<IPagedResult<AnnouncementListDto>>;

public class SearchAnnouncementsQueryHandler(IAnnouncementRepository announcementRepository)
    : IQueryHandler<SearchAnnouncementsQuery, IPagedResult<AnnouncementListDto>>
{
    public async Task<IPagedResult<AnnouncementListDto>> Handle(SearchAnnouncementsQuery query, CancellationToken cancellationToken)
    {
        var status = ParseEnumOrDefault<AnnouncementStatus>(query.Status);

        return await announcementRepository
            .AsQueryable()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.Keyword), c => c.Title.Contains(query.Keyword!) || c.Summary.Contains(query.Keyword!))
            .WhereIf(status.HasValue, c => c.Status == status!.Value)
            .OrderByDescending(c => c.CreatedAt)
            .ProjectToType<AnnouncementListDto>()
            .ToPagedResultAsync(query.Current, query.PageSize, cancellationToken);
    }

    private static TEnum? ParseEnumOrDefault<TEnum>(string? value)
        where TEnum : struct, Enum
    {
        return Enum.TryParse<TEnum>(value, ignoreCase: true, out var result)
            ? result
            : null;
    }
}
