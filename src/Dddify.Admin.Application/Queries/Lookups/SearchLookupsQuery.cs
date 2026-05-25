using Dddify.Admin.Application.Dtos.Lookups;

namespace Dddify.Admin.Application.Queries.Lookups;

public record SearchLookupsQuery(
    int Current,
    int PageSize,
    string? Code,
    string? Name) : IQuery<IPagedResult<LookupDto>>;

public class SearchLookupsQueryHandler(ILookupRepository lookupRepository) : IQueryHandler<SearchLookupsQuery, IPagedResult<LookupDto>>
{
    public async Task<IPagedResult<LookupDto>> Handle(SearchLookupsQuery query, CancellationToken cancellationToken)
    {
        return await lookupRepository
            .AsQueryable()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.Name), c => c.Name.Contains(query.Name!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.Code), c => c.Code == query.Code)
            .OrderBy(c => c.CreatedAt)
            .ProjectToType<LookupDto>()
            .ToPagedResultAsync(query.Current, query.PageSize, cancellationToken);
    }
}