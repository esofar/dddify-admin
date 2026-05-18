namespace Dddify.Admin.Application.Dtos.Departments;

/// <summary>
/// 部门列表项。
/// </summary>
/// <param name="Id">部门 ID。</param>
/// <param name="ParentId">上级部门 ID。</param>
/// <param name="Name">部门名称。</param>
/// <param name="FullName">部门全称。</param>
/// <param name="Type">部门类型。</param>
/// <param name="Leader">部门负责人。</param>
/// <param name="IsEnabled">是否启用。</param>
/// <param name="Order">排序值。</param>
public record DepartmentListDto(
    Guid Id,
    Guid? ParentId,
    string Name,
    string FullName,
    string? Type,
    DepartmentLeaderDto Leader,
    bool IsEnabled,
    int Order);
