using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Domain.Aggregates.Users;
using Dddify.Primitives;

namespace Dddify.Admin.Application.Queries.Users;

public record SearchUsersQuery(
    int Current,
    int PageSize,
    string? Name,
    string? Email,
    string? PhoneNumber,
    Guid? DepartmentId,
    Guid? RoleId,
    string? Gender,
    string? Status) : IQuery<IPagedResult<UserListDto>>;

public class SearchUsersQueryHandler(IUserRepository userRepository) : IQueryHandler<SearchUsersQuery, IPagedResult<UserListDto>>
{
    public async Task<IPagedResult<UserListDto>> Handle(SearchUsersQuery query, CancellationToken cancellationToken)
    {
        var gender = ParseEnumOrDefault<UserGender>(query.Gender);
        var status = ParseEnumOrDefault<UserStatus>(query.Status);

        return await userRepository
            .AsQueryable()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.Name), c => c.Name.Contains(query.Name!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.Email), c => c.Email.Contains(query.Email!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.PhoneNumber), c => c.PhoneNumber.Contains(query.PhoneNumber!))
            .WhereIf(query.DepartmentId.HasValue, c => c.Department.Id == query.DepartmentId)
            .WhereIf(query.RoleId.HasValue, c => c.Roles.Any(r => r.RoleId == query.RoleId))
            .WhereIf(gender.HasValue, c => c.Gender == gender!.Value)
            .WhereIf(status.HasValue, c => c.Status == status!.Value)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new UserListDto(
                c.Id,
                c.Name,
                c.NickName,
                c.Avatar,
                c.Email,
                c.PhoneNumber,
                c.BirthDate,
                c.Gender.ToString(),
                c.Status.ToString(),
                new UserDepartmentDto(c.Department.Id, c.Department.Name),
                c.Roles.Select(r => new UserRoleDto(r.RoleId, r.RoleName, r.IsCurrent))))
            .ToPagedResultAsync(query.Current, query.PageSize, cancellationToken);
    }

    private static TEnum? ParseEnumOrDefault<TEnum>(string? value)
        where TEnum : struct, Enum
    {
        return Enum.TryParse<TEnum>(value, ignoreCase: true, out var result)
            ? result
            : null;
    }
}
