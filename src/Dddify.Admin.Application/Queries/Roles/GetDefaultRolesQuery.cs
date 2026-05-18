using Dddify.Admin.Application.Dtos.Roles;

namespace Dddify.Admin.Application.Queries.Roles;

public record GetDefaultRolesQuery : IQuery<IEnumerable<RoleListDto>>;

public class GetDefaultRolesQueryHandler(IRoleRepository roleRepository) : IQueryHandler<GetDefaultRolesQuery, IEnumerable<RoleListDto>>
{
    public async Task<IEnumerable<RoleListDto>> Handle(GetDefaultRolesQuery query, CancellationToken cancellationToken)
    {
        var roles = await roleRepository.GetListAsync(c => c.IsDefault, cancellationToken);

        return roles
            .OrderBy(c => c.Order)
            .Adapt<IEnumerable<RoleListDto>>();
    }
}