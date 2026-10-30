namespace Dddify.Admin.Application.Abstractions.Authentication;

/// <summary>
/// 访问令牌服务。
/// </summary>
public interface IAccessTokenService
{
    /// <summary>
    /// 生成访问令牌。
    /// </summary>
    /// <param name="userId">用户ID。</param>
    /// <returns>JWT 格式访问令牌字符串。</returns>
    string Generate(Guid userId);
}
