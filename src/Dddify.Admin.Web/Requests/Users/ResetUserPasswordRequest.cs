namespace Dddify.Admin.Web.Requests.Users;

/// <summary>
/// 重置用户密码请求。
/// </summary>
/// <param name="NewPassword">新密码。</param>
public sealed record ResetUserPasswordRequest(string NewPassword);
