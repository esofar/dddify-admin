namespace Dddify.Admin.Application.Abstractions.Locking;

/// <summary>
/// 分布式锁服务。
/// </summary>
public interface IDistributedLock
{
    /// <summary>
    /// 获取指定资源的分布式锁。
    /// </summary>
    /// <param name="resource">加锁资源名称。</param>
    /// <param name="leaseTime">锁租约时长，未续约时到期失效。</param>
    /// <param name="timeout">最长等待时间；为 null 时持续等待，为零时仅尝试一次。</param>
    /// <param name="autoRenew">是否自动续约，默认开启。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>锁句柄，异步释放时解除锁。</returns>
    /// <exception cref="TimeoutException">等待超时仍未获取锁。</exception>
    Task<IDistributedLockHandle> AcquireAsync(
        string resource,
        TimeSpan leaseTime,
        TimeSpan? timeout = null,
        bool autoRenew = true,
        CancellationToken cancellationToken = default);
}
