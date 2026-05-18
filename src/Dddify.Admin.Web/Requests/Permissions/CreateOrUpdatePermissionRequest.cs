namespace Dddify.Admin.Web.Requests.Permissions;

/// <summary>
/// 新增或修改权限请求。
/// </summary>
/// <param name="ParentId">上级权限ID。</param>
/// <param name="Code">权限标识。</param>
/// <param name="Name">权限名称。</param>
/// <param name="Type">权限类型。</param>
/// <param name="Order">排序值。</param>
public sealed record CreateOrUpdatePermissionRequest(
    Guid? ParentId,
    string Code,
    string Name,
    string Type,
    int Order);
