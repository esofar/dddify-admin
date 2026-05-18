namespace Dddify.Admin.Application.Common.Locking;

/// <summary>
/// 提供跨进程、跨实例的分布式互斥锁能力。
/// </summary>
public interface IDistributedLock
{
    /// <summary>
    /// 尝试获取指定资源的分布式锁。
    /// </summary>
    /// <param name="resource">需要加锁的资源名称，同一资源在同一时间只允许一个持有者。</param>
    /// <param name="leaseTime">锁租约时长，超过该时间未续约时锁会自动过期。</param>
    /// <param name="timeout">获取锁的最长等待时间；为 <see cref="TimeSpan.Zero" /> 时表示不等待。</param>
    /// <param name="autoRenew">是否在持有锁期间自动续约，适用于执行时间可能超过租约的任务。</param>
    /// <param name="cancellationToken">取消等待或获取锁操作的令牌。</param>
    /// <returns>成功获取后的锁句柄，释放或释放资源时会解除锁。</returns>
    Task<IDistributedLockHandle> AcquireAsync(
        string resource,
        TimeSpan leaseTime,
        TimeSpan? timeout = null,
        bool autoRenew = true,
        CancellationToken cancellationToken = default);
}
