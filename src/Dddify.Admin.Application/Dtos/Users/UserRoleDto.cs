namespace Dddify.Admin.Application.Dtos.Users;

/// <summary>
/// 用户角色。
/// </summary>
/// <param name="RoleId">角色 ID。</param>
/// <param name="RoleName">角色名称。</param>
/// <param name="IsCurrent">是否为当前角色。</param>
public record UserRoleDto(Guid RoleId, string RoleName, bool IsCurrent);
