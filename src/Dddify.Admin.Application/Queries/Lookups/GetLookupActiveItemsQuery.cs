using Dddify.Admin.Application.Dtos.Lookups;
using Dddify.Admin.Application.Exceptions.Lookups;

namespace Dddify.Admin.Application.Queries.Lookups;

public record GetLookupActiveItemsQuery(string Code) : IQuery<IEnumerable<LookupActiveItemDto>>;

public class GetLookupEnableItemsQueryHandler(ILookupRepository lookupRepository, IDistributedCache distributedCache)
    : IQueryHandler<GetLookupActiveItemsQuery, IEnumerable<LookupActiveItemDto>>
{
    public async Task<IEnumerable<LookupActiveItemDto>> Handle(GetLookupActiveItemsQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Lookup.Items(query.Code);

        return await distributedCache.GetOrCreateJsonAsync(cacheKey, async () =>
        {
            var lookup = await lookupRepository.GetLookupWithItemsAsync(query.Code, cancellationToken)
                ?? throw new LookupNotFoundException(query.Code);

            return lookup.Items
                .Where(c => c.IsEnabled)
                .OrderBy(c => c.Order)
                .Adapt<IEnumerable<LookupActiveItemDto>>();
        },
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        },
        cancellationToken);
    }
}
