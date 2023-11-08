namespace Dddify.Admin.Host.Models.Dictionaries;

public class UpdateDictionaryRequest
{
    /// <summary>
    /// 字典名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 字典描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 并发标识
    /// </summary>
    public string ConcurrencyStamp { get; set; }
}