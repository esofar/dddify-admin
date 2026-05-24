namespace Dddify.Admin.Application.Services.Locking;

/// <summary>
/// 表示一次已获取的分布式锁持有句柄。
/// </summary>
public interface IDistributedLockHandle : IAsyncDisposable
{
    /// <summary>
    /// 当前锁保护的资源名称。
    /// </summary>
    string Resource { get; }

    /// <summary>
    /// 当前锁持有者的唯一令牌，用于确保只释放自己持有的锁。
    /// </summary>
    string Token { get; }

    /// <summary>
    /// 指示当前句柄是否仍然持有锁。
    /// </summary>
    bool IsAcquired { get; }

    /// <summary>
    /// 主动释放当前持有的分布式锁。
    /// </summary>
    /// <param name="cancellationToken">取消释放操作的令牌。</param>
    ValueTask ReleaseAsync(CancellationToken cancellationToken = default);
}
