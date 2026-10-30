namespace Dddify.Admin.Application.Caching;

/// <summary>
/// 收集当前工作单元提交后需要删除的缓存键。
/// </summary>
public sealed class CacheInvalidationQueue
{
    private readonly HashSet<string> keys = new(StringComparer.Ordinal);

    public void Enqueue(string key) => keys.Add(key);

    public string[] Drain()
    {
        var pending = keys.ToArray();
        keys.Clear();
        return pending;
    }

    public void Clear() => keys.Clear();
}
