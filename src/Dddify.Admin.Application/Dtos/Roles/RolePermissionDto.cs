namespace Dddify.Admin.Application.Dtos.Roles;

/// <summary>
/// 角色权限。
/// </summary>
/// <param name="PermissionId">权限 ID。</param>
/// <param name="PermissionCode">权限标识。</param>
public record RolePermissionDto(string PermissionId, string PermissionCode);
