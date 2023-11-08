namespace Dddify.Admin.Host.Models.Dictionaries;

public class CreateDictionaryRequest
{
    /// <summary>
    /// 字典编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 字典名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 字典描述
    /// </summary>
    public string? Description { get; set; }
}