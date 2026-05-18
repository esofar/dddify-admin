namespace Dddify.Admin.Web.Requests.Auths;

/// <summary>
/// 手机验证码登录请求。
/// </summary>
/// <param name="PhoneNumber">手机号。</param>
/// <param name="Code">短信验证码。</param>
/// <param name="DeviceId">设备ID。</param>
/// <param name="DeviceName">设备名称。</param>
/// <param name="RememberMe">是否保持登录。</param>
public sealed record SmsLoginRequest(
    string PhoneNumber,
    string Code,
    string DeviceId,
    string DeviceName,
    bool RememberMe);
