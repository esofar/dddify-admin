using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Exceptions.Users;

namespace Dddify.Admin.Application.Queries.Users;

public record GetUserRolesQuery(Guid UserId) : IQuery<IEnumerable<UserRoleDto>>;

public class GetUserRolesQueryHandler(
    IUserRepository userRepository,
    IMapper mapper) : IQueryHandler<GetUserRolesQuery, IEnumerable<UserRoleDto>>
{
    public async Task<IEnumerable<UserRoleDto>> Handle(GetUserRolesQuery query, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithRolesAsync(query.UserId, cancellationToken)
            ?? throw new UserNotFoundException(query.UserId);

        return mapper.Map<IEnumerable<UserRoleDto>>(user.Roles);
    }
}
