namespace Dddify.Admin.Host.Models.Dictionaries;

public class UpdateDictionaryItemRequest
{
    /// <summary>
    /// 字典项名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 并发戳
    /// </summary>
    public string ConcurrencyStamp { get; set; }
}