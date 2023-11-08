using Dddify.Admin.Application.Dtos.Dictionaries;
using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Queries.Dictionaries;

public record GetItemsByDictionaryCodeQuery(string DictionaryCode) : IQuery<IEnumerable<DictionaryItemDto>>;

public class GetItemsByDictionaryCodeQueryHandler : IQueryHandler<GetItemsByDictionaryCodeQuery, IEnumerable<DictionaryItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private IDistributedCache _distributedCache;

    public GetItemsByDictionaryCodeQueryHandler(IApplicationDbContext context, IMapper mapper, IDistributedCache distributedCache)
    {
        _context = context;
        _mapper = mapper;
        _distributedCache = distributedCache;
    }

    public async Task<IEnumerable<DictionaryItemDto>> Handle(GetItemsByDictionaryCodeQuery query, CancellationToken cancellationToken)
    {
        var dict = await _context.Dictionaries
            .Include(c => c.Items.OrderBy(x => x.Order))
            .FirstOrDefaultAsync(c => c.Code == query.DictionaryCode, cancellationToken);

        if (dict is null)
        {
            throw new DictionaryNotFoundException(query.DictionaryCode);
        }

        var key = CacheKeys.Dictionary(dict.Id);


        //if (_distributedCache.Get(key))
        //{

        //}

        return _mapper.Map<IEnumerable<DictionaryItemDto>>(dict.Items);
    }
}