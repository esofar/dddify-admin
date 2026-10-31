namespace Dddify.Admin.Application.Abstractions.Identifiers;

/// <summary>
/// GUID 生成器。
/// </summary>
public interface IGuidGenerator
{
    /// <summary>
    /// 创建 GUID。
    /// </summary>
    /// <returns>新生成的 GUID。</returns>
    Guid Create();
}
