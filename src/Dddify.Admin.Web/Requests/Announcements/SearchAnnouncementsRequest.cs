namespace Dddify.Admin.Web.Requests.Announcements;

/// <summary>
/// 查询公告列表请求。
/// </summary>
/// <param name="Current">当前页码。</param>
/// <param name="PageSize">每页条数。</param>
/// <param name="Keyword">标题或摘要关键字。</param>
/// <param name="Status">公告状态。</param>
public record SearchAnnouncementsRequest(
    int Current = 1,
    int PageSize = 20,
    string? Keyword = null,
    string? Status = null);
