using Dddify.Admin.Domain.Events.Announcements;
using Dddify.Admin.Domain.Exceptions.Announcements;

namespace Dddify.Admin.Domain.Aggregates.Announcements;

public class Announcement : AuditableAggregateRoot<Guid>
{
    public const int MaxTitleLength = 100;
    public const int MaxSummaryLength = 500;

    public string Title { get; private set; } = default!;

    public string Summary { get; private set; } = default!;

    public AnnouncementContent Content { get; private set; } = default!;

    public AnnouncementAudience Audience { get; private set; } = default!;

    public AnnouncementStatus Status { get; private set; }

    public DateTimeOffset? PublishedAt { get; private set; }

    public Guid? PublishedBy { get; private set; }

    public DateTimeOffset? WithdrawnAt { get; private set; }

    public Guid? WithdrawnBy { get; private set; }

    private Announcement() { }

    public Announcement(
        Guid id,
        string title,
        string summary,
        AnnouncementContent content,
        AnnouncementAudience audience)
    {
        Id = id;
        ChangeTitle(title);
        ChangeSummary(summary);
        Content = content;
        Audience = audience;
        Status = AnnouncementStatus.Draft;
    }

    public void Change(string title, string summary, AnnouncementContent content, AnnouncementAudience audience)
    {
        EnsureDraft();

        ChangeTitle(title);
        ChangeSummary(summary);
        Content = content;
        Audience = audience;
    }

    public void Publish(Guid publishedBy, DateTimeOffset publishedAt)
    {
        EnsureDraft();

        Status = AnnouncementStatus.Published;
        PublishedAt = publishedAt;
        PublishedBy = publishedBy;

        AddDomainEvent(new AnnouncementPublishedDomainEvent(Id, Title, Summary, Audience));
    }

    public void Withdraw(Guid withdrawnBy, DateTimeOffset withdrawnAt)
    {
        if (Status != AnnouncementStatus.Published)
        {
            throw new AnnouncementStatusInvalidException(Id, Status, AnnouncementStatus.Published);
        }

        Status = AnnouncementStatus.Withdrawn;

        WithdrawnAt = withdrawnAt;
        WithdrawnBy = withdrawnBy;

        AddDomainEvent(new AnnouncementWithdrawnDomainEvent(Id, withdrawnAt));
    }

    public void EnsureDelete()
    {
        if (Status == AnnouncementStatus.Published)
        {
            throw new AnnouncementCannotDeletePublishedException(Id);
        }
    }

    private void EnsureDraft()
    {
        if (Status != AnnouncementStatus.Draft)
        {
            throw new AnnouncementStatusInvalidException(Id, Status, AnnouncementStatus.Draft);
        }
    }

    private void ChangeTitle(string title)
    {
        Title = title.Trim();
    }

    private void ChangeSummary(string summary)
    {
        Summary = summary.Trim();
    }
}
