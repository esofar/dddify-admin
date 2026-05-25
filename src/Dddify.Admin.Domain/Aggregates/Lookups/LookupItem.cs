using Dddify.Admin.Domain.Exceptions.Lookups;

namespace Dddify.Admin.Domain.Aggregates.Lookups;

public class LookupItem : AuditableEntity<Guid>
{
    public const int MaxValueLength = 20;
    public const int MaxLabelLength = 20;
    public const int MaxColorLength = 20;

    public Guid LookupId { get; private set; }

    public string Value { get; private set; } = default!;

    public string Label { get; private set; } = default!;

    public string? Color { get; private set; }

    public int Order { get; private set; }

    public bool IsPreset { get; private set; }

    public bool IsEnabled { get; private set; }

    private LookupItem() { }

    public LookupItem(Guid lookupId, Guid itemId, string value, string label, string? color)
    {
        LookupId = lookupId;
        Id = itemId;
        Value = value;
        Label = label;
        Color = color;
        IsPreset = false;
        IsEnabled = true;
    }

    public void Change(string label, string? color)
    {
        Label = label;
        Color = color;
    }

    public void Sort(int order)
    {
        Order = order;
    }

    public void Enable()
    {
        if (IsEnabled)
        {
            throw new LookupItemAlreadyEnabledException(LookupId, Id);
        }

        IsEnabled = true;
    }

    public void Disable()
    {
        if (!IsEnabled)
        {
            throw new LookupItemAlreadyDisabledException(LookupId, Id);
        }

        IsEnabled = false;
    }
}
