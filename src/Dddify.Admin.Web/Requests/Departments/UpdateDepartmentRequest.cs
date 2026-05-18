namespace Dddify.Admin.Web.Requests.Departments;

/// <summary>
/// 修改部门请求。
/// </summary>
/// <param name="ParentId">上级部门ID。</param>
/// <param name="Name">部门名称。</param>
/// <param name="Type">部门类型。</param>
/// <param name="LeaderId">部门负责人ID。</param>
/// <param name="IsEnabled">是否启用。</param>
/// <param name="Order">排序值。</param>
/// <param name="ConcurrencyStamp">并发标记。</param>
public sealed record UpdateDepartmentRequest(
    Guid? ParentId,
    string Name,
    string Type,
    Guid LeaderId,
    bool IsEnabled,
    int Order,
    string? ConcurrencyStamp);
