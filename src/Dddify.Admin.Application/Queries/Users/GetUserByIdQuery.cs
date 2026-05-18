using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Exceptions.Users;

namespace Dddify.Admin.Application.Queries.Users;

public record GetUserByIdQuery(Guid Id) : IQuery<UserDetailDto>;

public class GetUserByIdQueryHandler(IUserRepository userRepository) : IQueryHandler<GetUserByIdQuery, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(query.Id, cancellationToken)
            ?? throw new UserNotFoundException(query.Id);

        return new UserDetailDto(
            user.Id,
            user.Name,
            user.NickName,
            user.Avatar,
            user.Email,
            user.PhoneNumber,
            user.BirthDate,
            user.Gender.ToString(),
            user.Status.ToString(),
            new UserDepartmentDto(user.Department.Id, user.Department.Name),
            user.Roles.Select(role => new UserRoleDto(role.RoleId, role.RoleName, role.IsCurrent)),
            user.ConcurrencyStamp);
    }
}
