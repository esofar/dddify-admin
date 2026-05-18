namespace Dddify.Admin.Application.Dtos.Users;

/// <summary>
/// 当前登录用户。
/// </summary>
/// <param name="Id">用户 ID。</param>
/// <param name="Name">姓名。</param>
/// <param name="NickName">昵称。</param>
/// <param name="Avatar">头像。</param>
/// <param name="Email">邮箱。</param>
/// <param name="PhoneNumber">手机号。</param>
/// <param name="CurrentRoleId">当前角色 ID。</param>
/// <param name="Roles">角色列表。</param>
/// <param name="Permissions">权限列表。</param>
public record CurrentUserDto(
    Guid Id,
    string Name,
    string? NickName,
    string? Avatar,
    string Email,
    string PhoneNumber,
    Guid CurrentRoleId,
    IEnumerable<UserRoleDto> Roles,
    IEnumerable<string> Permissions);
