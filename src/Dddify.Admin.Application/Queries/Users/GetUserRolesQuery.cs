using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Exceptions.Users;

namespace Dddify.Admin.Application.Queries.Users;

public record GetUserRolesQuery(Guid UserId) : IQuery<IEnumerable<UserRoleDto>>;

public class GetUserRolesQueryHandler(IUserRepository userRepository, IDistributedCache distributedCache) : IQueryHandler<GetUserRolesQuery, IEnumerable<UserRoleDto>>
{
    public async Task<IEnumerable<UserRoleDto>> Handle(GetUserRolesQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.User.Roles(query.UserId);

        return await distributedCache.GetOrCreateJsonAsync(cacheKey, async () =>
        {
            var user = await userRepository.GetUserWithRolesAsync(query.UserId, cancellationToken)
                ?? throw new UserNotFoundException(query.UserId);

            return user.Roles.Adapt<IEnumerable<UserRoleDto>>();
        },
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        },
        cancellationToken);
    }
}
