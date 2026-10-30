using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Exceptions.Users;

namespace Dddify.Admin.Application.Queries.Users;

// 鉴权调用应设置 UseCache = false，展示查询默认使用缓存。
public record GetUserRolesQuery(Guid UserId, bool UseCache = true) : IQuery<IEnumerable<UserRoleDto>>;

public class GetUserRolesQueryHandler(
    IUserRepository userRepository,
    IDistributedCache distributedCache,
    IMapper mapper) : IQueryHandler<GetUserRolesQuery, IEnumerable<UserRoleDto>>
{
    public async Task<IEnumerable<UserRoleDto>> Handle(GetUserRolesQuery query, CancellationToken cancellationToken)
    {
        async Task<IEnumerable<UserRoleDto>> LoadAsync()
        {
            var user = await userRepository.GetUserWithRolesAsync(query.UserId, cancellationToken)
                ?? throw new UserNotFoundException(query.UserId);

            return mapper.Map<IEnumerable<UserRoleDto>>(user.Roles);
        }

        if (!query.UseCache)
        {
            return await LoadAsync();
        }

        return await distributedCache.GetOrCreateJsonAsync(CacheKeys.UserRoles(query.UserId), LoadAsync,
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        },
        cancellationToken);
    }
}
