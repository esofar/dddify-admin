namespace Dddify.Admin.Host.Models.Dictionaries;

public class SortDictionaryItemRequest
{
    /// <summary>
    /// 排序后的字典选项ID集合
    /// </summary>
    public Guid[] DictionaryItemIds { get; set; }
}