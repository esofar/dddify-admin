namespace Dddify.Admin.Application.Services.Security;

/// <summary>
/// 密码生成器。
/// </summary>
public interface IPasswordGenerator
{
    /// <summary>
    /// 生成符合安全要求的随机密码。
    /// </summary>
    /// <param name="length">密码长度，默认 12 位。</param>
    /// <param name="includeSpecialCharacters">是否包含特殊字符。</param>
    /// <returns>随机生成的明文密码。</returns>
    string Generate(int length = 12, bool includeSpecialCharacters = true);
}
