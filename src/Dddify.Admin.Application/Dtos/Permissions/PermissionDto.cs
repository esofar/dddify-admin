namespace Dddify.Admin.Application.Dtos.Permissions;

/// <summary>
/// 权限。
/// </summary>
/// <param name="Id">权限 ID。</param>
/// <param name="ParentId">上级权限 ID。</param>
/// <param name="Code">权限标识。</param>
/// <param name="Name">权限名称。</param>
/// <param name="Type">权限类型。</param>
/// <param name="Order">排序值。</param>
public record PermissionDto(Guid Id, Guid? ParentId, string Code, string Name, string Type, int Order);
