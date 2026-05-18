namespace Dddify.Admin.Application.Dtos.Departments;

/// <summary>
/// 部门负责人。
/// </summary>
/// <param name="Id">用户 ID。</param>
/// <param name="Name">姓名。</param>
public record DepartmentLeaderDto(Guid Id, string Name);
