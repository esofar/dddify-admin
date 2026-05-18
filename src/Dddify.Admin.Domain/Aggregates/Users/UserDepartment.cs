namespace Dddify.Admin.Domain.Aggregates.Users;

/// <summary>
/// 用户所属部门值对象。
/// </summary>
/// <param name="id">部门 ID。</param>
/// <param name="name">部门名称。</param>
public sealed class UserDepartment(Guid id, string name) : ValueObject
{
    /// <summary>
    /// 部门名称最大长度。
    /// </summary>
    public const int MaxNameLength = 50;

    /// <summary>
    /// 部门 ID。
    /// </summary>
    public Guid Id { get; private set; } = id;

    /// <summary>
    /// 部门名称。
    /// </summary>
    public string Name { get; private set; } = name;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return Name;
    }
}
