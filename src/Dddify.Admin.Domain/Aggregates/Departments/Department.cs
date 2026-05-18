using Dddify.Admin.Domain.Events.Departments;

namespace Dddify.Admin.Domain.Aggregates.Departments;

public class Department : AuditableAggregateRoot<Guid>, IHasConcurrencyStamp
{
    public const int MaxNameLength = 50;
    public const int MaxCodeLength = 20;
    public const int MaxFullNameLength = 1000;
    public const int MaxPathLength = 100;
    public const int MaxTypeLength = 20;

    public Guid? ParentId { get; private set; }

    public string Name { get; private set; } = default!;

    public string Code { get; private set; } = default!;

    public string FullName { get; private set; } = default!;

    public string Path { get; private set; } = default!;

    public int Level { get; private set; }

    public string Type { get; private set; } = default!;

    public DepartmentLeader Leader { get; private set; } = default!;

    public bool IsEnabled { get; private set; }

    public int Order { get; private set; }

    public string? ConcurrencyStamp { get; set; }

    private Department() { }

    private Department(
        Guid id,
        string name,
        string code,
        Guid? parentId,
        string type,
        Guid leaderId,
        string leaderName,
        bool isEnabled,
        int order,
        string path,
        int level,
        string fullName)
    {
        Id = id;
        Name = name;
        Code = code;
        ParentId = parentId;
        Type = type;
        IsEnabled = isEnabled;
        Order = order;
        Path = path;
        Level = level;
        FullName = fullName;

        Leader = new DepartmentLeader(leaderId, leaderName);
    }

    public static Department CreateRoot(
        Guid id,
        string name,
        string code,
        string type,
        Guid leaderId,
        string leaderName,
        bool isEnabled = true,
        int order = 0)
    {
        var path = $"/{code}/";
        return new Department(
            id,
            name,
            code,
            null,
            type,
            leaderId,
            leaderName,
            isEnabled,
            order,
            path,
            0,
            name);
    }

    public Department CreateChild(
        Guid id,
        string name,
        string code,
        string type,
        Guid leaderId,
        string leaderName,
        bool isEnabled = true,
        int order = 0)
    {
        var childPath = $"{Path}{code}/";
        var childFullName = $"{FullName}/{name}";
        var child = new Department(
            id,
            name,
            code,
            Id,
            type,
            leaderId,
            leaderName,
            isEnabled,
            order,
            childPath,
            Level + 1,
            childFullName);

        AddDomainEvent(new DepartmentCreatedDomainEvent(child.Id, child.ParentId, child.Path));
        return child;
    }

    public void ChangeLeader(Guid leaderId, string leaderName)
    {
        var leader = new DepartmentLeader(leaderId, leaderName);

        if (Leader != leader)
        {
            Leader = leader;
            AddDomainEvent(new DepartmentLeaderChangedDomainEvent(Id, leaderId, leaderName));
        }
    }

    public void ChangeName(string newName)
    {
        if (newName != Name)
        {
            Name = newName;
            AddDomainEvent(new DepartmentRenamedDomainEvent(Id, newName));
        }
    }

    public void MoveTo(Guid? newParentId, string newParentPath, string? newParentFullName, int newParentLevel)
    {
        var oldFullName = FullName;
        var oldPath = Path;

        ParentId = newParentId;
        Level = newParentId is null ? 0 : newParentLevel + 1;
        Path = newParentId is null ? $"/{Code}/" : $"{newParentPath}{Code}/";
        FullName = newParentId is null ? Name : $"{newParentFullName}/{Name}";

        AddDomainEvent(new DepartmentMovedDomainEvent(Id, oldPath, Path, oldFullName, FullName));
    }

    public void ChangeOrder(int order)
    {
        Order = order;
    }

    public void Enable()
    {
        if (!IsEnabled)
        {
            IsEnabled = true;
            AddDomainEvent(new DepartmentEnabledDomainEvent(Id));
        }
    }

    public void Disable()
    {
        if (IsEnabled)
        {
            IsEnabled = false;
            AddDomainEvent(new DepartmentDisabledDomainEvent(Id));
        }
    }

    public void ApplyHierarchyRefresh(string path, int level, string fullName)
    {
        Path = path;
        Level = level;
        FullName = fullName;
    }
}
