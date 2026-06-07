namespace Dddify.Admin.Application.Dtos.Announcements;

public record AnnouncementAudienceTargetDto(Guid Id, string Name);

public record AnnouncementAudienceDto
{
    public string Type { get; set; } = default!;

    public IEnumerable<Guid> TargetIds { get; set; } = [];

    public IEnumerable<AnnouncementAudienceTargetDto> Targets { get; set; } = [];
}
