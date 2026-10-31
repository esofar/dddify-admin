namespace Dddify.Admin.Application.Abstractions.Security;

/// <summary>
/// 密码生成器。
/// </summary>
public interface IPasswordGenerator
{
    /// <summary>
    /// 生成随机密码。
    /// </summary>
    /// <param name="length">密码长度，范围 8～16，默认 12。</param>
    /// <param name="includeSpecialCharacters">是否包含特殊字符。</param>
    /// <returns>随机明文密码。</returns>
    string Generate(int length = 12, bool includeSpecialCharacters = true);
}
