namespace Dddify.Admin.Application.Abstractions.Authentication;

/// <summary>
/// 访问令牌签发服务。
/// </summary>
public interface IAccessTokenService
{
    /// <summary>
    /// 为指定用户签发访问令牌。
    /// </summary>
    /// <param name="userId">用户 ID。</param>
    /// <returns>已签名的 JWT 访问令牌。</returns>
    string Generate(Guid userId);
}
