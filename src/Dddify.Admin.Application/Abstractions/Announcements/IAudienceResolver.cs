using Dddify.Admin.Application.Dtos.Announcements;
using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Application.Abstractions.Announcements;

/// <summary>
/// 公告受众解析器。
/// </summary>
public interface IAudienceResolver
{
    /// <summary>
    /// 按批次解析公告接收用户 ID。
    /// </summary>
    /// <param name="audience">公告受众配置。</param>
    /// <param name="batchSize">每批最大用户数。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>用户 ID 批次序列。</returns>
    IAsyncEnumerable<IReadOnlyCollection<Guid>> ResolveUserIdsAsync(
        AnnouncementAudience audience,
        int batchSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解析用于展示的受众目标信息。
    /// </summary>
    /// <param name="audience">公告受众配置。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>受众目标集合。</returns>
    Task<IReadOnlyCollection<AnnouncementAudienceTargetDto>> ResolveDisplayTargetsAsync(
        AnnouncementAudience audience,
        CancellationToken cancellationToken = default);
}
