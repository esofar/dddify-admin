using Dddify.Admin.Application.Dtos.Permissions;

namespace Dddify.Admin.Application.Queries.Permissions;

public record GetAllPermissionsQuery : IQuery<IEnumerable<PermissionDto>>;

public class GetAllPermissionsQueryHandler(IPermissionRepository permissionRepository) : IQueryHandler<GetAllPermissionsQuery, IEnumerable<PermissionDto>>
{
    public async Task<IEnumerable<PermissionDto>> Handle(GetAllPermissionsQuery query, CancellationToken cancellationToken)
    {
        var permissions = await permissionRepository.GetAllAsync(cancellationToken);

        return permissions
            .OrderBy(c => c.Order)
            .Select(c => new PermissionDto(c.Id, c.ParentId, c.Code, c.Name, c.Type.ToString(), c.Order));
    }
}
