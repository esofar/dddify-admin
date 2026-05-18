namespace Dddify.Admin.Web.Requests.Users;

/// <summary>
/// 新增用户请求。
/// </summary>
/// <param name="Password">密码。</param>
/// <param name="Name">姓名。</param>
/// <param name="NickName">昵称。</param>
/// <param name="Gender">性别。</param>
/// <param name="BirthDate">出生日期。</param>
/// <param name="Email">邮箱。</param>
/// <param name="PhoneNumber">手机号。</param>
/// <param name="DepartmentId">部门ID。</param>
public sealed record CreateUserRequest(
    string Password,
    string Name,
    string? NickName,
    string Gender,
    DateOnly? BirthDate,
    string Email,
    string PhoneNumber,
    Guid DepartmentId);
