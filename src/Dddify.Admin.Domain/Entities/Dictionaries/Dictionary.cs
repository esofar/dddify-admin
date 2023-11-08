using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Domain.Entities.Dictionaries;

public class Dictionary : FullAuditedAggregateRoot<Guid>, IHasConcurrencyStamp, ISoftDeletable
{
    public Dictionary(string code, string name, string? description)
    {
        Code = code;
        Name = name;
        Description = description;
    }

    private Dictionary() { }

    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsDeleted { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public List<DictionaryItem> Items { get; private set; }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public void SortItems(Guid[] orderedIds)
    {
        for (int order = 1; order <= orderedIds.Length; order++)
        {
            Items.Single(c => c.Id == orderedIds[order]).Sort(order);
        }
    }

    public void AddItem(string code, string name, DictionaryItemType type)
    {
        if (Items.Any(c => c.Code == code))
        {
            throw new DictionaryItemCodeDuplicateException(code);
        }

        if (Items.Any(c => c.Name == name))
        {
            throw new DictionaryItemNameDuplicateException(name);
        }

        Items.Add(new(code, name, type));
    }

    public void DeleteItem(Guid id)
    {
        var dictionaryItem = Items.SingleOrDefault(c => c.Id == id);

        if (dictionaryItem is null)
        {
            throw new DictionaryItemNotFoundException(id);
        }

        Items.Remove(dictionaryItem);
    }
}