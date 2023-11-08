using Dddify.Admin.Application.Dtos.Dictionaries;

namespace Dddify.Admin.Application.Queries.Dictionaries;

public record GetDictionariesQuery(
    int Page,
    int Size,
    string? Code,
    string? Name) : IQuery<IPagedResult<DictionaryDto>>;

public class GetDictionariesQueryHandler :IQueryHandler<GetDictionariesQuery, IPagedResult<DictionaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDictionariesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IPagedResult<DictionaryDto>> Handle(GetDictionariesQuery query, CancellationToken cancellationToken)
    {
        return await _context.Dictionaries
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.Code), c => EF.Functions.Like(c.Code, $"{query.Code}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(query.Name), c => EF.Functions.Like(c.Name, $"{query.Name}%"))
            .OrderByDescending(c => c.CreatedAt)
            .ProjectToType<DictionaryDto>()
            .ToPagedResultAsync(query.Page, query.Size, cancellationToken);
    }
}