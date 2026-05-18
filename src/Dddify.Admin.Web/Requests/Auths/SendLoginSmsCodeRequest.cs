namespace Dddify.Admin.Web.Requests.Auths;

/// <summary>
/// 发送登录短信验证码请求。
/// </summary>
/// <param name="PhoneNumber">手机号。</param>
public sealed record SendLoginSmsCodeRequest(string PhoneNumber);
