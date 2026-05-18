namespace Dddify.Admin.Web.Requests.Auths;

/// <summary>
/// 账号密码登录请求。
/// </summary>
/// <param name="Account">账号，支持邮箱或手机号。</param>
/// <param name="Password">密码。</param>
/// <param name="DeviceId">设备ID。</param>
/// <param name="DeviceName">设备名称。</param>
/// <param name="RememberMe">是否保持登录。</param>
public sealed record AccountLoginRequest(
    string Account,
    string Password,
    string DeviceId,
    string DeviceName,
    bool RememberMe);
