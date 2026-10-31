namespace Dddify.Admin.Web.Requests.InboxItems;

/// <summary>
/// 标记消息已读请求。
/// </summary>
/// <param name="Ids">消息 ID 列表。</param>
public record MarkInboxItemsAsReadRequest(Guid[] Ids);
