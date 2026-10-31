namespace Dddify.Admin.Application.Abstractions.Authentication;

/// <summary>
/// 刷新令牌服务。
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// 生成刷新令牌及其哈希和有效期信息。
    /// </summary>
    /// <param name="now">当前时间。</param>
    /// <returns>刷新令牌生成结果。</returns>
    GeneratedRefreshToken Generate(DateTimeOffset now);

    /// <summary>
    /// 计算刷新令牌哈希。
    /// </summary>
    /// <param name="token">刷新令牌明文。</param>
    /// <returns>用于存储和查找的令牌哈希。</returns>
    string Hash(string token);
}

/// <summary>
/// 刷新令牌生成结果。
/// </summary>
/// <param name="Value">返回客户端的令牌明文。</param>
/// <param name="Hash">用于存储的令牌哈希。</param>
/// <param name="ExpiresAt">过期时间。</param>
/// <param name="RefreshTokenDays">有效天数。</param>
public sealed record GeneratedRefreshToken(
    string Value,
    string Hash,
    DateTimeOffset ExpiresAt,
    int RefreshTokenDays);
