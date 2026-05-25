namespace Dddify.Admin.Web.Requests.Roles;

/// <summary>
/// 修改角色请求。
/// </summary>
/// <param name="Name">角色名称。</param>
/// <param name="IsDefault">是否默认角色。</param>
/// <param name="Order">排序值。</param>
/// <param name="Description">角色描述。</param>
/// <param name="ConcurrencyStamp">并发标记。</param>
public sealed record UpdateRoleRequest(
    string Name,
    bool IsDefault,
    int Order,
    string? Description,
    string? ConcurrencyStamp);
