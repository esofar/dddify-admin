namespace Dddify.Admin.Application.Dtos.Auths;

/// <summary>
/// 登录令牌。
/// </summary>
/// <param name="AccessToken">访问令牌。</param>
/// <param name="RefreshToken">刷新令牌。</param>
/// <param name="IsPersistent">是否持久化登录状态。</param>
public record TokenDto(string AccessToken, string RefreshToken, bool IsPersistent);
