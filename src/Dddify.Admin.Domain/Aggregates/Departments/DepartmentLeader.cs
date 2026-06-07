namespace Dddify.Admin.Domain.Aggregates.Departments;

/// <summary>
/// 部门负责人值对象
/// </summary>
public sealed class DepartmentLeader(Guid id, string name) : ValueObject
{
    public const int MaxNameLength = 50;

    /// <summary>
    /// 负责人ID
    /// </summary>
    public Guid Id { get; private set; } = id;

    /// <summary>
    /// 负责人姓名
    /// </summary>
    public string Name { get; private set; } = name.Trim();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return Name;
    }
}
