namespace Dddify.Admin.Application.Services.Security;

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
    /// 验证明文密码是否与密码哈希匹配。
    /// </summary>
    /// <param name="password">明文密码。</param>
    /// <param name="passwordHash">密码哈希值。</param>
    /// <returns>匹配返回 <see langword="true" />，否则返回 <see langword="false" />。</returns>
    bool Verify(string password, string passwordHash);
}
