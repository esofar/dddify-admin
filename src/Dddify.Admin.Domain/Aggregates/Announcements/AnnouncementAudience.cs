namespace Dddify.Admin.Domain.Aggregates.Announcements;

public sealed class AnnouncementAudience : ValueObject
{
    public AnnouncementAudienceType Type { get; private set; }

    public IEnumerable<Guid> TargetIds { get; private set; } = default!;

    private AnnouncementAudience() { }

    public AnnouncementAudience(AnnouncementAudienceType type, IEnumerable<Guid> targetIds)
    {
        Type = type;
        TargetIds = targetIds ?? [];
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;

        foreach (var targetId in TargetIds)
        {
            yield return targetId;
        }
    }
}
