namespace Dddify.Admin.Application.Abstractions.Authentication;

/// <summary>
/// 刷新令牌服务，负责生成凭据、计算哈希及过期时间。
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// 生成高强度随机刷新令牌。
    /// </summary>
    /// <returns>刷新令牌明文。</returns>
    string Generate();

    /// <summary>
    /// 对刷新令牌进行哈希。
    /// </summary>
    /// <param name="token">刷新令牌明文。</param>
    /// <returns>刷新令牌哈希。</returns>
    string Hash(string token);

    /// <summary>
    /// 计算刷新令牌过期时间。
    /// </summary>
    /// <param name="now">当前时间。</param>
    /// <returns>刷新令牌过期时间。</returns>
    DateTimeOffset GetExpiresAt(DateTimeOffset now);
}
