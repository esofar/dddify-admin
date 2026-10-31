namespace Dddify.Admin.Application.Abstractions.Locking;

/// <summary>
/// 分布式锁句柄。
/// </summary>
public interface IDistributedLockHandle : IAsyncDisposable
{
    /// <summary>
    /// 加锁资源名称。
    /// </summary>
    string Resource { get; }

    /// <summary>
    /// 锁持有者令牌，用于校验释放权限。
    /// </summary>
    string Token { get; }

    /// <summary>
    /// 本地记录的锁持有状态，不实时查询锁是否有效。
    /// </summary>
    bool IsAcquired { get; }

    /// <summary>
    /// 释放锁并停止续约。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    ValueTask ReleaseAsync(CancellationToken cancellationToken = default);
}
