namespace Dddify.Admin.Domain.Entities.Organizations;

public class Organization : FullAuditedAggregateRoot<Guid>, IHasConcurrencyStamp, ISoftDeletable
{
    public Organization(Guid? parentId, string name, string type, int order, bool isEnabled)
    {
        ParentId = parentId;
        Name = name;
        Type = type;
        Order = order;
        IsEnabled = isEnabled;
    }

    private Organization() { }

    public Guid? ParentId { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    public int Order { get; private set; }
    public bool IsEnabled { get; private set; }
    public bool IsDeleted { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public List<User> Users { get; set; }

    public bool CanBeDeleted => !Users.Any();

    public void Update(Guid? parentId, string name, string type, int order, bool isEnabled)
    {
        ParentId = parentId;
        Name = name;
        Type = type;
        Order = order;
        IsEnabled = isEnabled;
    }
}