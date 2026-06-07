namespace Dddify.Admin.Web.Requests.Announcements;

/// <summary>
/// 创建公告请求。
/// </summary>
/// <param name="Title">公告标题。</param>
/// <param name="Summary">公告摘要。</param>
/// <param name="ContentHtml">公告内容 HTML。</param>
/// <param name="AudienceType">公告受众类型。</param>
/// <param name="AudienceTargetIds">公告受众目标 ID 列表。</param>
public record CreateAnnouncementRequest(
    string Title,
    string Summary,
    string ContentHtml,
    string AudienceType,
    IEnumerable<Guid>? AudienceTargetIds);
