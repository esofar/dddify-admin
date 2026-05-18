using Dddify.Admin.Application.Dtos.Roles;

namespace Dddify.Admin.Application.Queries.Roles;

public record GetAllRolesQuery : IQuery<IEnumerable<RoleListDto>>;

public class GetAllRolesQueryHandler(IRoleRepository roleRepository) : IQueryHandler<GetAllRolesQuery, IEnumerable<RoleListDto>>
{
    public async Task<IEnumerable<RoleListDto>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
    {
        var roles = await roleRepository.GetAllAsync(cancellationToken);

        return roles
            .OrderBy(c => c.Order)
            .Adapt<IEnumerable<RoleListDto>>();
    }
}