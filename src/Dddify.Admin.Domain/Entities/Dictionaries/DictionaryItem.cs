namespace Dddify.Admin.Domain.Entities.Dictionaries;

public class DictionaryItem : FullAuditedEntity<Guid>, IHasConcurrencyStamp, ISoftDeletable
{
    public DictionaryItem(string code, string name, DictionaryItemType type)
    {
        Code = code;
        Name = name;
        Type = type;
    }

    private DictionaryItem() { }

    public string Code { get; private set; }
    public string Name { get; private set; }
    public DictionaryItemType Type { get; private set; }
    public int Order { get; private set; }
    public bool IsEnabled { get; private set; }
    public bool IsDeleted { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public Guid DictionaryId { get; private set; }
    public Dictionary Dictionary { get; private set; }

    public void SetName(string newName)
        => Name = newName;

    public void Sort(int order)
        => Order = order;
}