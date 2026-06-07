namespace Dddify.Admin.Application.Dtos.Announcements;

public record AnnouncementDetailDto(
    Guid Id,
    string Title,
    string Summary,
    AnnouncementContentDto Content,
    AnnouncementAudienceDto Audience,
    string Status,
    DateTimeOffset? PublishedAt,
    Guid? PublishedBy,
    DateTimeOffset? WithdrawnAt,
    Guid? WithdrawnBy,
    DateTimeOffset? CreatedAt);
