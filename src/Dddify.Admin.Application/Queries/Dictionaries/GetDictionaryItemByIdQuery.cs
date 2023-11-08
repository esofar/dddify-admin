using Dddify.Admin.Application.Dtos.Dictionaries;
using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Queries.Dictionaries;

public record GetDictionaryItemByIdQuery(
    Guid DictionaryId,
    Guid DictionaryItemId) : IQuery<DictionaryItemDetailDto>;

public class GetDictionaryItemByIdQueryHandler : IQueryHandler<GetDictionaryItemByIdQuery, DictionaryItemDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetDictionaryItemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DictionaryItemDetailDto> Handle(GetDictionaryItemByIdQuery query, CancellationToken cancellationToken)
    {
        var dictionary = await _context.Dictionaries
            .AsNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == query.DictionaryId, cancellationToken);

        if (dictionary is null)
        {
            throw new DictionaryNotFoundException(query.DictionaryId);
        }

        var dictionaryItem = dictionary.Items.FirstOrDefault(c => c.Id == query.DictionaryItemId);

        if (dictionaryItem is null)
        {
            throw new DictionaryItemNotFoundException(query.DictionaryItemId);
        }

        return dictionaryItem.Adapt<DictionaryItemDetailDto>();
    }
}