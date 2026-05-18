namespace Dddify.Admin.Web.Requests.Users;

/// <summary>
/// 查询用户列表请求。
/// </summary>
/// <param name="Current">当前页码。</param>
/// <param name="PageSize">每页数量。</param>
/// <param name="Name">姓名。</param>
/// <param name="Email">邮箱。</param>
/// <param name="PhoneNumber">手机号。</param>
/// <param name="DepartmentId">部门ID。</param>
/// <param name="RoleId">角色ID。</param>
/// <param name="Gender">性别。</param>
/// <param name="Status">状态。</param>
public sealed record SearchUserRequest(
    int Current,
    int PageSize,
    string? Name,
    string? Email,
    string? PhoneNumber,
    Guid? DepartmentId,
    Guid? RoleId,
    string? Gender,
    string? Status);
