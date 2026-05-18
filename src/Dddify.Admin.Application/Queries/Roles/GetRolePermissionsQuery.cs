using Dddify.Admin.Application.Dtos.Roles;
using Dddify.Admin.Application.Exceptions.Roles;

namespace Dddify.Admin.Application.Queries.Roles;

public record GetRolePermissionsQuery(Guid RoleId) : IQuery<IEnumerable<RolePermissionDto>>;

public class GetRolePermissionsQueryHandler(IRoleRepository roleRepository, IDistributedCache distributedCache) : IQueryHandler<GetRolePermissionsQuery, IEnumerable<RolePermissionDto>>
{
    public async Task<IEnumerable<RolePermissionDto>> Handle(GetRolePermissionsQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Role.Permissions(query.RoleId);

        return await distributedCache.GetOrCreateJsonAsync(cacheKey, async () =>
        {
            var role = await roleRepository.GetRoleWithPermissionsAsync(query.RoleId, cancellationToken)
                ?? throw new RoleNotFoundException(query.RoleId);

            return role.Permissions.Adapt<IEnumerable<RolePermissionDto>>();
        },
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        },
        cancellationToken);
    }
}
