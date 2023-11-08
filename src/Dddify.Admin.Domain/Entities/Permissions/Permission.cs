namespace Dddify.Admin.Domain.Entities.Permissions;

/// <summary>
/// 权限表
/// </summary>
public class Permission : FullAuditedAggregateRoot<Guid>, IHasConcurrencyStamp, ISoftDeletable
{
    public Permission(string code, string name, int order)
    {
        Code = code;
        Name = name;
        Order = order;
    }

    private Permission() { }

    /// <summary>
    /// 关联权限
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string Code { get; set; } = default!;

    /// <summary>
    /// 权限名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 权限类型
    /// </summary>
    public PermissionType Type { get; set; }

    /// <summary>
    /// 展示顺序
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; private set; } = true;

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 并发戳
    /// </summary>
    public string? ConcurrencyStamp { get; set; }
}