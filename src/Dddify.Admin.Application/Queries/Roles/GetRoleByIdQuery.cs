using Dddify.Admin.Application.Dtos.Roles;
using Dddify.Admin.Domain.Exceptions.Roles;

namespace Dddify.Admin.Application.Queries.Roles;

public record GetRoleByIdQuery(Guid Id) : IQuery<RoleDetailDto>;

public class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, RoleDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetRoleByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoleDetailDto> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (role is null)
        {
            throw new RoleNotFoundException(query.Id);
        }

        return role.Adapt<RoleDetailDto>();
    }
}