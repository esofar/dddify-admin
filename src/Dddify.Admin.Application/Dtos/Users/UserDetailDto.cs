namespace Dddify.Admin.Application.Dtos.Users;

/// <summary>
/// 用户详情。
/// </summary>
/// <param name="Id">用户 ID。</param>
/// <param name="Name">姓名。</param>
/// <param name="NickName">昵称。</param>
/// <param name="Avatar">头像。</param>
/// <param name="Email">邮箱。</param>
/// <param name="PhoneNumber">手机号。</param>
/// <param name="BirthDate">出生日期。</param>
/// <param name="Gender">性别。</param>
/// <param name="Status">状态。</param>
/// <param name="Department">部门。</param>
/// <param name="Roles">角色列表。</param>
/// <param name="ConcurrencyStamp">并发标记。</param>
public record UserDetailDto(
    Guid Id,
    string Name,
    string? NickName,
    string? Avatar,
    string Email,
    string PhoneNumber,
    DateOnly? BirthDate,
    string Gender,
    string Status,
    UserDepartmentDto Department,
    IEnumerable<UserRoleDto> Roles,
    string? ConcurrencyStamp)
    : UserListDto(Id, Name, NickName, Avatar, Email, PhoneNumber, BirthDate, Gender, Status, Department, Roles);
