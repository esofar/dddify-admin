namespace Dddify.Admin.Application.Dtos.InboxItems;

public record InboxItemListDto(
    Guid Id,
    string Title,
    string Summary,
    InboxItemSourceDto Source,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset? CreatedAt);
