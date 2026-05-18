namespace Dddify.Admin.Application.Dtos.Users;

/// <summary>
/// 用户所属部门。
/// </summary>
/// <param name="Id">部门 ID。</param>
/// <param name="Name">部门名称。</param>
public record UserDepartmentDto(Guid Id, string Name);
