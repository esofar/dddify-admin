namespace Dddify.Admin.Web.Requests.InboxItems;

/// <summary>
/// 查询收件箱列表请求。
/// </summary>
/// <param name="Current">当前页码。</param>
/// <param name="PageSize">每页条数。</param>
/// <param name="IsRead">是否已读。</param>
public record SearchInboxItemsRequest(
    int Current = 1,
    int PageSize = 20,
    bool? IsRead = null);
