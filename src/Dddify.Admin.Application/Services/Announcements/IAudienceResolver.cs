using Dddify.Admin.Application.Dtos.Announcements;
using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Application.Services.Announcements;

/// <summary>
/// 公告受众解析器。
/// </summary>
public interface IAudienceResolver
{
    /// <summary>
    /// 将公告受众解析为实际接收公告的用户 ID，并按批次返回。
    /// </summary>
    /// <param name="audience">公告受众配置。</param>
    /// <param name="batchSize">每批返回的用户 ID 数量。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>按批次返回的用户 ID 集合。</returns>
    IAsyncEnumerable<IReadOnlyCollection<Guid>> ResolveUserIdsAsync(
        AnnouncementAudience audience,
        int batchSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将公告受众配置中的目标 ID 解析为用于详情展示和表单回显的目标信息。
    /// </summary>
    /// <param name="audience">公告受众配置。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>可展示的受众目标集合。</returns>
    Task<IReadOnlyCollection<AnnouncementAudienceTargetDto>> ResolveDisplayTargetsAsync(
        AnnouncementAudience audience,
        CancellationToken cancellationToken = default);
}
