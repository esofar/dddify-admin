using Dddify.Admin.Application.Dtos.Announcements;
using Dddify.Admin.Domain.Aggregates.InboxItems;

namespace Dddify.Admin.Application.Dtos.InboxItems;

public record InboxItemSourceDetailDto(
    InboxItemSourceType SourceType,
    Guid SourceId,
    bool IsAvailable,
    string? UnavailableReason,
    AnnouncementDetailDto? Announcement);
