namespace Dddify.Admin.Application.Dtos.Announcements;

/// <summary>
/// 公告列表项 DTO。
/// </summary>
/// <param name="Id">公告 ID。</param>
/// <param name="Title">标题。</param>
/// <param name="Summary">摘要。</param>
/// <param name="Status">状态。</param>
/// <param name="AudienceType">受众类型。</param>
/// <param name="PublishedAt">发布时间。</param>
/// <param name="WithdrawnAt">撤回时间。</param>
/// <param name="CreatedAt"></param>
public record AnnouncementListDto(
    Guid Id,
    string Title,
    string Summary,
    string Status,
    string AudienceType,
    DateTimeOffset? PublishedAt,
    DateTimeOffset? WithdrawnAt,
    DateTimeOffset? CreatedAt);
