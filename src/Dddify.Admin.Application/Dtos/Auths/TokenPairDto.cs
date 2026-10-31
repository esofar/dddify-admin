namespace Dddify.Admin.Application.Dtos.Auths;

/// <summary>
/// 访问令牌与刷新令牌。
/// </summary>
/// <param name="AccessToken">访问令牌。</param>
/// <param name="RefreshToken">刷新令牌。</param>
/// <param name="IsPersistent">是否持久化登录状态。</param>
/// <param name="RefreshTokenDays">刷新令牌有效天数。</param>
public record TokenPairDto(string AccessToken, string RefreshToken, bool IsPersistent, int RefreshTokenDays);
