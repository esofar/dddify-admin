using Dddify.Admin.Application.Dtos.Roles;

namespace Dddify.Admin.Application.Queries.Roles;

public record GetRolesByIdsQuery(IEnumerable<Guid> Ids) : IQuery<IEnumerable<RoleListDto>>;

public class GetRolesByIdsQueryHandler(IRoleRepository roleRepository) : IQueryHandler<GetRolesByIdsQuery, IEnumerable<RoleListDto>>
{
    public async Task<IEnumerable<RoleListDto>> Handle(GetRolesByIdsQuery query, CancellationToken cancellationToken)
    {
        var roles = await roleRepository.GetListAsync(c => query.Ids.Contains(c.Id), cancellationToken);

        return roles
            .OrderBy(c => c.Order)
            .Adapt<IEnumerable<RoleListDto>>();
    }
}