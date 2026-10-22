namespace Dddify.Admin.Web.Requests.Auths;

/// <summary>
/// 邮箱验证码登录请求。
/// </summary>
/// <param name="Email">邮箱账号。</param>
/// <param name="Code">六位邮箱验证码。</param>
/// <param name="DeviceId">设备ID。</param>
/// <param name="DeviceName">设备名称。</param>
/// <param name="RememberMe">是否保持登录。</param>
public sealed record VerificationCodeLoginRequest(
    string Email,
    string Code,
    string DeviceId,
    string DeviceName,
    bool RememberMe);
