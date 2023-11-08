namespace Dddify.Admin.Domain.Entities.Users;

/// <summary>
/// 用户角色关联表
/// </summary>
public class UserRole : Entity
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public Guid RoleId { get; set; }

    public Role? Role { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { UserId, RoleId };
    }
}