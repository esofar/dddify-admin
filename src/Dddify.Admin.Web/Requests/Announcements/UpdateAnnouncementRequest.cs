namespace Dddify.Admin.Web.Requests.Announcements;

/// <summary>
/// 修改公告请求。
/// </summary>
/// <param name="Title">公告标题。</param>
/// <param name="Summary">公告摘要。</param>
/// <param name="ContentHtml">公告正文 HTML。</param>
/// <param name="AudienceType">受众类型。</param>
/// <param name="AudienceTargetIds">受众目标 ID 列表。</param>
public record UpdateAnnouncementRequest(
    string Title,
    string Summary,
    string ContentHtml,
    string AudienceType,
    IEnumerable<Guid>? AudienceTargetIds);
