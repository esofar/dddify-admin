namespace Dddify.Admin.Application.Services;

/// <summary>
/// 令牌服务，用于生成访问令牌、刷新令牌和刷新令牌哈希。
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// 生成访问令牌。
    /// </summary>
    /// <param name="userId">用户ID。</param>
    /// <returns>JWT 格式访问令牌字符串。</returns>
    string GenerateAccessToken(Guid userId);

    /// <summary>
    /// 生成高强度随机刷新令牌。
    /// </summary>
    /// <returns>刷新令牌明文，仅用于返回给调用方写入 HttpOnly Cookie。</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// 获取刷新令牌过期时间。
    /// </summary>
    /// <param name="now">当前时间。</param>
    /// <returns>刷新令牌过期时间。</returns>
    DateTimeOffset GetRefreshTokenExpiresAt(DateTimeOffset now);

    /// <summary>
    /// 对刷新令牌进行哈希。
    /// </summary>
    /// <param name="refreshToken">刷新令牌明文。</param>
    /// <returns>刷新令牌哈希。</returns>
    string HashRefreshToken(string refreshToken);
}
