namespace Dddify.Admin.Web.Requests.Auths;

/// <summary>
/// 切换当前角色请求。
/// </summary>
/// <param name="RoleId">角色ID。</param>
public sealed record SwitchUserCurrentRoleRequest(Guid RoleId);
