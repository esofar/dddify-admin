using Dddify.Admin.Application.Dtos.Roles;
using Dddify.Admin.Application.Exceptions.Roles;

namespace Dddify.Admin.Application.Queries.Roles;

// 鉴权调用应设置 UseCache = false，展示查询默认使用缓存。
public record GetRolePermissionsQuery(Guid RoleId, bool UseCache = true) : IQuery<IEnumerable<RolePermissionDto>>;

public class GetRolePermissionsQueryHandler(
    IRoleRepository roleRepository,
    IDistributedCache distributedCache) : IQueryHandler<GetRolePermissionsQuery, IEnumerable<RolePermissionDto>>
{
    public async Task<IEnumerable<RolePermissionDto>> Handle(GetRolePermissionsQuery query, CancellationToken cancellationToken)
    {
        async Task<IEnumerable<RolePermissionDto>> LoadAsync()
        {
            var role = await roleRepository.GetRoleWithPermissionsAsync(query.RoleId, cancellationToken)
                ?? throw new RoleNotFoundException(query.RoleId);

            return role.Permissions.Adapt<IEnumerable<RolePermissionDto>>();
        }

        if (!query.UseCache)
        {
            return await LoadAsync();
        }

        return await distributedCache.GetOrCreateJsonAsync(CacheKeys.RolePermissions(query.RoleId), LoadAsync,
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        },
        cancellationToken);
    }
}
