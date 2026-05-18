namespace Dddify.Admin.Web.Requests.Auths;

/// <summary>
/// 退出登录请求。
/// </summary>
/// <param name="DeviceId">设备ID。</param>
public sealed record LogoutRequest(string DeviceId);
