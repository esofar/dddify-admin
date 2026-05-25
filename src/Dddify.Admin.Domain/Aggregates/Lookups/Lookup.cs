using Dddify.Admin.Domain.Exceptions.Lookups;

namespace Dddify.Admin.Domain.Aggregates.Lookups;

/// <summary>
/// 字典聚合根
/// </summary>
public class Lookup : AuditableAggregateRoot<Guid>
{
    public const int MaxCodeLength = 20;
    public const int MaxNameLength = 20;
    public const int MaxDescriptionLength = 100;

    private readonly List<LookupItem> _items = [];

    public string Code { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public IReadOnlyCollection<LookupItem> Items => _items.AsReadOnly();

    private Lookup() { }

    public Lookup(Guid id, string code, string name, string? description)
    {
        Id = id;
        Code = code;
        Name = name;
        Description = description;
    }

    public void Change(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public void EnsureCanDelete()
    {
        if (_items.Count > 0)
        {
            throw new LookupHasItemsException(Id);
        }
    }

    public void AddItem(Guid itemId, string value, string label, string? color)
    {
        if (_items.Any(c => c.Value == value || c.Label == label))
        {
            throw new LookupItemDuplicateException(Id, value, label);
        }

        _items.Add(new LookupItem(Id, itemId, value, label, color));
    }

    public void ChangeItem(Guid itemId, string label, string? color)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new LookupItemNotFoundException(Id, itemId);

        if (_items.Any(c => c.Label == label && c.Id != itemId))
        {
            throw new LookupItemDuplicateException(Id, null, label);
        }

        item.Change(label, color);
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new LookupItemNotFoundException(Id, itemId);

        if (item.IsPreset)
        {
            throw new LookupItemCannotDeletedException(Id, itemId);
        }

        _items.Remove(item);
    }

    public void SortItems(Guid[] orderedIds)
    {
        for (int order = 0; order < orderedIds.Length; order++)
        {
            var item = _items.FirstOrDefault(c => c.Id == orderedIds[order]);
            item?.Sort(order + 1);
        }
    }

    public void EnableItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new LookupItemNotFoundException(Id, itemId);

        item.Enable();
    }

    public void DisableItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId)
             ?? throw new LookupItemNotFoundException(Id, itemId);

        item.Disable();
    }
}
