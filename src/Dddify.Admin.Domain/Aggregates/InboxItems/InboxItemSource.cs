namespace Dddify.Admin.Domain.Aggregates.InboxItems;

public sealed class InboxItemSource : ValueObject
{
    public InboxItemSourceType Type { get; private set; }

    public Guid Id { get; private set; }

    private InboxItemSource() { }

    public InboxItemSource(InboxItemSourceType type, Guid id)
    {
        Type = type;
        Id = id;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
        yield return Id;
    }
}
