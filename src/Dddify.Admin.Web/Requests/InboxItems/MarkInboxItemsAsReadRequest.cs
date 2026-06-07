namespace Dddify.Admin.Web.Requests.InboxItems;

/// <summary>
/// 标记收件箱消息为已读请求。
/// </summary>
/// <param name="Ids">消息ID。</param>
public record MarkInboxItemsAsReadRequest(Guid[] Ids);
