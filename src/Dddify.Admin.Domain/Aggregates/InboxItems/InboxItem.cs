using Dddify.Admin.Domain.Exceptions.InboxItems;

namespace Dddify.Admin.Domain.Aggregates.InboxItems;

public class InboxItem : AuditableAggregateRoot<Guid>
{
    public const int MaxTitleLength = 100;
    public const int MaxSummaryLength = 500;

    public Guid UserId { get; private set; }

    public string Title { get; private set; } = default!;

    public string Summary { get; private set; } = default!;

    public InboxItemSource Source { get; private set; } = default!;

    public bool IsRead { get; private set; }

    public DateTimeOffset? ReadAt { get; private set; }

    private InboxItem() { }

    public InboxItem(
        Guid id,
        Guid userId,
        string title,
        string summary,
        InboxItemSource source)
    {
        Id = id;
        UserId = userId;
        ChangeTitle(title);
        ChangeSummary(summary);
        Source = source;
    }

    public void MarkAsRead(DateTimeOffset readAt)
    {
        EnsureDelete();

        IsRead = true;
        ReadAt = readAt;
    }

    public void EnsureDelete()
    {
        if (IsDeleted)
        {
            throw new InboxItemAlreadyDeletedException(Id);
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
