using Dddify.Admin.Application.Dtos.Permissions;

namespace Dddify.Admin.Application.Queries.Permissions;

public record GetPermissionsByIdsQuery(IEnumerable<Guid> Ids) : IQuery<IEnumerable<PermissionDto>>;

public class GetPermissionsByIdsQueryHandler(IPermissionRepository permissionRepository) : IQueryHandler<GetPermissionsByIdsQuery, IEnumerable<PermissionDto>>
{
    public async Task<IEnumerable<PermissionDto>> Handle(GetPermissionsByIdsQuery query, CancellationToken cancellationToken)
    {
        var permissions = await permissionRepository.GetListAsync(c => query.Ids.Contains(c.Id), cancellationToken);

        return permissions
            .OrderBy(c => c.Order)
            .Adapt<IEnumerable<PermissionDto>>();
    }
}