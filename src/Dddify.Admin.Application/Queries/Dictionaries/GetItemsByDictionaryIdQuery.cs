using Dddify.Admin.Application.Dtos.Dictionaries;
using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Queries.Dictionaries;

public record GetItemsByDictionaryIdQuery(Guid DictionaryId) : IQuery<IEnumerable<DictionaryItemDto>>;

public class GetItemsByDictionaryIdQueryHandler : IQueryHandler<GetItemsByDictionaryIdQuery, IEnumerable<DictionaryItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetItemsByDictionaryIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DictionaryItemDto>> Handle(GetItemsByDictionaryIdQuery query, CancellationToken cancellationToken)
    {
        var dictionary = await _context.Dictionaries
           .Include(c => c.Items.OrderBy(x => x.Order))
           .FirstOrDefaultAsync(c => c.Id == query.DictionaryId, cancellationToken);

        if (dictionary is null)
        {
            throw new DictionaryNotFoundException(query.DictionaryId);
        }

        return _mapper.Map<IEnumerable<DictionaryItemDto>>(dictionary.Items);
    }
}