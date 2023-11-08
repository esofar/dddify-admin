namespace Dddify.Admin.Domain.Entities.Roles;

public class Role : FullAuditedAggregateRoot<Guid>, IHasConcurrencyStamp, ISoftDeletable
{
    public Role(string code, string name, string? description)
    {
        Code = code;
        Name = name;
        Description = description;
    }

    private Role() { }

    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? ConcurrencyStamp { get; set; }
    public bool IsDeleted { get; set; }
    public List<User> Users { get; set; }
    public List<UserRole> UserRoles { get; set; }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}