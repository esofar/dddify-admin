using Dddify.Admin.Application.Dtos.Organizations;

namespace Dddify.Admin.Application.Queries.Organizations;

public record GetOrganizationsQuery(
    string? Name,
    string? Type,
    bool? IsEnabled) : IQuery<IList<OrganizationDto>>;

public class GetOrganizationsQueryHandler : IQueryHandler<GetOrganizationsQuery, IList<OrganizationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrganizationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IList<OrganizationDto>> Handle(GetOrganizationsQuery query, CancellationToken cancellationToken)
    {
        return await _context.Organizations
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.Name), c => EF.Functions.Like(c.Name, $"{query.Name}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(query.Type), c => c.Type == query.Type)
            .WhereIf(query.IsEnabled.HasValue, c => c.IsEnabled == query.IsEnabled)
            .OrderBy(c => c.Order)
            .ProjectToType<OrganizationDto>()
            .ToListAsync(cancellationToken);
    }
}