namespace Dddify.Admin.Domain.Constants;

/// <summary>
/// 缓存建
/// </summary>
public static class CacheKeys
{
    /// <summary>
    /// 指定字典
    /// </summary>
    public static Func<Guid, string> Dictionary => new(id => $"dictionaries:{id}");
}
