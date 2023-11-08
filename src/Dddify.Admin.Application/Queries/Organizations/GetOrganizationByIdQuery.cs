using Dddify.Admin.Application.Dtos.Organizations;
using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Queries.Organizations;

public record GetOrganizationByIdQuery(Guid Id) : IQuery<OrganizationDetailDto>;

public class GetOrganizationByIdQueryHandler : IQueryHandler<GetOrganizationByIdQuery, OrganizationDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetOrganizationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizationDetailDto> Handle(GetOrganizationByIdQuery query, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (organization is null)
        {
            throw new OrganizationNotFoundException(query.Id);
        }

        return organization.Adapt<OrganizationDetailDto>();
    }
}