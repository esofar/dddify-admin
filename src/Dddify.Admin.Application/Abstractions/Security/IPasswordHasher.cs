namespace Dddify.Admin.Application.Abstractions.Security;

/// <summary>
/// 密码哈希服务。
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// 生成密码哈希。
    /// </summary>
    /// <param name="password">明文密码。</param>
    /// <returns>密码哈希值。</returns>
    string Hash(string password);

    /// <summary>
    /// 验证明文密码与哈希是否匹配。
    /// </summary>
    /// <param name="password">明文密码。</param>
    /// <param name="passwordHash">密码哈希值。</param>
    /// <returns>是否匹配。</returns>
    bool Verify(string password, string passwordHash);
}
