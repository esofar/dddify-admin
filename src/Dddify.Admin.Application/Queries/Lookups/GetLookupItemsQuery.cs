using Dddify.Admin.Application.Dtos.Lookups;
using Dddify.Admin.Application.Exceptions.Lookups;

namespace Dddify.Admin.Application.Queries.Lookups;

public record GetLookupItemsQuery(Guid LookupId) : IQuery<IEnumerable<LookupItemDto>>;

public class GetLookupItemsQueryHandler(ILookupRepository lookupRepository) : IQueryHandler<GetLookupItemsQuery, IEnumerable<LookupItemDto>>
{
    public async Task<IEnumerable<LookupItemDto>> Handle(GetLookupItemsQuery query, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository
            .AsQueryable()
            .AsNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == query.LookupId, cancellationToken)
            ?? throw new LookupNotFoundException(query.LookupId);

        return lookup.Items
            .OrderBy(c => c.Order)
            .Adapt<IEnumerable<LookupItemDto>>();
    }
}