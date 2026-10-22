namespace Dddify.Admin.Web.Requests.Auths;

/// <summary>
/// 发送登录邮箱验证码请求。
/// </summary>
/// <param name="Email">邮箱账号。</param>
public sealed record SendEmailVerificationCodeRequest(string Email);
