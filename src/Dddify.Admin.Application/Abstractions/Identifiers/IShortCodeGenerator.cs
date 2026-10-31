namespace Dddify.Admin.Application.Abstractions.Identifiers;

/// <summary>
/// 短码生成器。
/// </summary>
public interface IShortCodeGenerator
{
    /// <summary>
    /// 根据 GUID 生成指定长度的短码。
    /// </summary>
    /// <param name="id">源 GUID。</param>
    /// <param name="length">短码长度，范围 1～11，默认 8。</param>
    /// <returns>由数字和大小写字母组成的短码。</returns>
    /// <remarks>相同参数生成相同短码，不保证不同 GUID 的短码唯一。</remarks>
    string Generate(Guid id, int length = 8);
}
