using Dddify.Admin.Application.Dtos.Dictionaries;
using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Queries.Dictionaries;

public record GetDictionaryByIdQuery(Guid Id) : IQuery<DictionaryDetailDto>;

public class GetDictionaryByIdQueryHandler : IQueryHandler<GetDictionaryByIdQuery, DictionaryDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetDictionaryByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DictionaryDetailDto> Handle(GetDictionaryByIdQuery query, CancellationToken cancellationToken)
    {
        var dictionary = await _context.Dictionaries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (dictionary is null)
        {
            throw new DictionaryNotFoundException(query.Id);
        }

        return dictionary.Adapt<DictionaryDetailDto>();
    }
}