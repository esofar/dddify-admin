using System.Text.Json;

namespace Dddify.Admin.Application.Caching;

/// <summary>
/// 分布式缓存扩展。
/// </summary>
/// <remarks>缓存和序列化异常由调用方处理。</remarks>
public static class DistributedCacheExtensions
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    /// <summary>
    /// 将非空值序列化为 JSON 并写入缓存。
    /// </summary>
    /// <typeparam name="T">缓存值类型。</typeparam>
    /// <param name="cache">缓存实例。</param>
    /// <param name="key">缓存键。</param>
    /// <param name="value">缓存值，不能为 null。</param>
    /// <param name="options">过期选项，默认 5 分钟绝对过期。</param>
    /// <param name="serializerOptions">JSON 序列化选项。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public static async Task SetJsonAsync<T>(
        this IDistributedCache cache,
        string key,
        T value,
        DistributedCacheEntryOptions? options = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);
        cancellationToken.ThrowIfCancellationRequested();

        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, serializerOptions);
        cancellationToken.ThrowIfCancellationRequested();

        await cache.SetAsync(key, bytes, options ?? new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = DefaultExpiration
        }, cancellationToken);
    }

    /// <summary>
    /// 读取并反序列化 JSON 缓存。
    /// </summary>
    /// <typeparam name="T">缓存值类型。</typeparam>
    /// <param name="cache">缓存实例。</param>
    /// <param name="key">缓存键。</param>
    /// <param name="serializerOptions">JSON 序列化选项。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>缓存值；未命中时返回类型默认值。</returns>
    public static async Task<T?> GetJsonAsync<T>(
        this IDistributedCache cache,
        string key,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        var (_, value) = await cache.TryGetJsonAsync<T>(key, serializerOptions, cancellationToken);
        return value;
    }

    /// <summary>
    /// 读取 JSON 缓存并返回命中状态。
    /// </summary>
    /// <typeparam name="T">缓存值类型。</typeparam>
    /// <param name="cache">缓存实例。</param>
    /// <param name="key">缓存键。</param>
    /// <param name="serializerOptions">JSON 序列化选项。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>命中状态及缓存值；未命中或数据为空时 Found 为 false。</returns>
    /// <remarks>JSON null 属于命中；无效 JSON 会抛出异常。</remarks>
    public static async Task<(bool Found, T? Value)> TryGetJsonAsync<T>(
        this IDistributedCache cache,
        string key,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        cancellationToken.ThrowIfCancellationRequested();

        var bytes = await cache.GetAsync(key, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        return bytes is not { Length: > 0 }
            ? (false, default)
            : (true, JsonSerializer.Deserialize<T>(bytes, serializerOptions));
    }

    /// <summary>
    /// 读取缓存，未命中时回源并缓存非空结果。
    /// </summary>
    /// <typeparam name="T">缓存值类型。</typeparam>
    /// <param name="cache">缓存实例。</param>
    /// <param name="key">缓存键。</param>
    /// <param name="factory">接收取消令牌的回源函数。</param>
    /// <param name="options">过期选项，默认 5 分钟绝对过期。</param>
    /// <param name="serializerOptions">JSON 序列化选项。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>缓存值或回源结果。</returns>
    /// <remarks>null 视为未命中且不缓存；不合并并发回源请求。</remarks>
    public static async Task<T> GetOrCreateJsonAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<CancellationToken, Task<T>> factory,
        DistributedCacheEntryOptions? options = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);
        cancellationToken.ThrowIfCancellationRequested();

        var (found, cached) = await cache.TryGetJsonAsync<T>(key, serializerOptions, cancellationToken);
        if (found && cached is not null)
        {
            return cached;
        }

        var result = await factory(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        if (result is not null)
        {
            await cache.SetJsonAsync(key, result, options, serializerOptions, cancellationToken);
        }

        return result;
    }

    /// <summary>
    /// 校验并去重缓存键后逐个删除。
    /// </summary>
    /// <param name="cache">缓存实例。</param>
    /// <param name="keys">待删除的缓存键。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>非原子操作；异常或取消时停止，已完成的删除不会回滚。</remarks>
    public static async Task RemoveManyAsync(
        this IDistributedCache cache,
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();

        var distinctKeys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var key in keys)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(keys));
            distinctKeys.Add(key);
        }

        foreach (var key in distinctKeys)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await cache.RemoveAsync(key, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
