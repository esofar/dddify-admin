namespace Dddify.Admin.Web.Requests.Auths;

/// <summary>
/// 邮箱密码登录请求。
/// </summary>
/// <param name="Email">邮箱账号。</param>
/// <param name="Password">密码。</param>
/// <param name="DeviceId">设备 ID。</param>
/// <param name="DeviceName">设备名称。</param>
/// <param name="IsPersistent">是否持久化登录状态。</param>
public sealed record PasswordLoginRequest(
    string Email,
    string Password,
    string DeviceId,
    string DeviceName,
    bool IsPersistent);
