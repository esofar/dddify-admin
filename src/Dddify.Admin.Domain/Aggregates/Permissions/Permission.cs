namespace Dddify.Admin.Domain.Aggregates.Permissions;

public class Permission : AuditableAggregateRoot<Guid>
{
    public const int MaxCodeLength = 50;
    public const int MaxNameLength = 50;

    public Guid? ParentId { get; set; }

    public string Code { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    public PermissionType Type { get; set; }

    public int Order { get; private set; }

    private Permission() { }

    public Permission(Guid id, Guid? parentId, string code, string name, PermissionType type, int order)
    {
        Id = id;
        ParentId = parentId;
        Code = code;
        Name = name;
        Type = type;
        Order = order;
    }

    public void Change(Guid? parentId, string code, string name, PermissionType type, int order)
    {
        ParentId = parentId;
        Code = code;
        Name = name;
        Type = type;
        Order = order;
    }
}
