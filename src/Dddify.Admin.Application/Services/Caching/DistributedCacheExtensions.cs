using System.Text.Json;

namespace Dddify.Admin.Application.Services.Caching;

/// <summary>
/// Provides JSON-based extension methods for <see cref="IDistributedCache"/>.
/// </summary>
public static class DistributedCacheExtensions
{
    /// <summary>
    /// Serializes the specified value as JSON and stores it in the distributed cache.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The cache key used to store the value.</param>
    /// <param name="value">The value to serialize and cache.</param>
    /// <param name="options">The cache entry options used to control expiration and other settings.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous cache operation.</returns>
    public static async Task SetJsonAsync<T>(
        this IDistributedCache cache,
        string key,
        T value,
        DistributedCacheEntryOptions? options = null,
        CancellationToken token = default)
    {
        await cache.SetStringAsync(key, JsonSerializer.Serialize(value), options ?? new DistributedCacheEntryOptions(), token);
    }

    /// <summary>
    /// Gets a cached JSON value and deserializes it to the specified type.
    /// </summary>
    /// <typeparam name="T">The target type to deserialize the cached value to.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The cache key used to retrieve the value.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The deserialized value if found; otherwise, <see langword="default"/>.</returns>
    public static async Task<T?> GetJsonAsync<T>(
        this IDistributedCache cache,
        string key,
        CancellationToken token = default)
    {
        var value = await cache.GetStringAsync(key, token);

        if (value == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value);
    }

    /// <summary>
    /// Gets a cached value by key, or creates, caches, and returns a new value when the key does not exist.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The cache key used to retrieve or store the value.</param>
    /// <param name="factory">The factory used to create the value when it is not found in the cache.</param>
    /// <param name="options">The cache entry options used to control expiration and other settings.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation and contains the cached or newly created value.</returns>
    public static async Task<T> GetOrCreateJsonAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<Task<T>> factory,
        DistributedCacheEntryOptions? options = null,
        CancellationToken token = default)
    {
        var cached = await cache.GetStringAsync(key, token);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<T>(cached)!;
        }

        var result = await factory();

        var value = JsonSerializer.Serialize(result);

        await cache.SetStringAsync(key, value, options ?? new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        }, token);

        return result;
    }
}
