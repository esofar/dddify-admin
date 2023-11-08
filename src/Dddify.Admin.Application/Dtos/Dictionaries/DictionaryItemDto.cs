namespace Dddify.Admin.Application.Dtos.Dictionaries;

public record DictionaryItemDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public DictionaryItemType Type { get; set; }
    public int Order { get; set; }
    public bool IsEnabled { get; set; }
}