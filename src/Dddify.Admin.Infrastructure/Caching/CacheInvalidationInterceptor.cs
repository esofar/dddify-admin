using Dddify.Admin.Application.Caching;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using System.Data.Common;

namespace Dddify.Admin.Infrastructure.Caching;

/// <summary>
/// 数据库事务成功提交后删除缓存，回滚时丢弃待删除的键。
/// </summary>
public sealed class CacheInvalidationInterceptor(
    CacheInvalidationQueue queue,
    IDistributedCache cache,
    ILogger<CacheInvalidationInterceptor> logger) : DbTransactionInterceptor
{
    public override void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
    {
        foreach (var key in queue.Drain())
        {
            try
            {
                cache.Remove(key);
            }
            catch (Exception exception)
            {
                // 数据库已经提交，缓存故障不能使调用方误以为业务操作失败。
                logger.LogError(exception, "Failed to invalidate cache {CacheKey} after transaction commit.", key);
            }
        }
    }

    public override async Task TransactionCommittedAsync(
        DbTransaction transaction,
        TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        foreach (var key in queue.Drain())
        {
            try
            {
                // 提交后即使请求已取消，也应继续尝试删除缓存。
                await cache.RemoveAsync(key, CancellationToken.None);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to invalidate cache {CacheKey} after transaction commit.", key);
            }
        }
    }

    public override void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData)
        => queue.Clear();

    public override Task TransactionRolledBackAsync(
        DbTransaction transaction,
        TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        queue.Clear();
        return Task.CompletedTask;
    }

    public override void TransactionFailed(DbTransaction transaction, TransactionErrorEventData eventData)
        => queue.Clear();

    public override Task TransactionFailedAsync(
        DbTransaction transaction,
        TransactionErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        queue.Clear();
        return Task.CompletedTask;
    }
}
