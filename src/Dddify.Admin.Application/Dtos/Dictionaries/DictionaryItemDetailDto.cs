namespace Dddify.Admin.Application.Dtos.Dictionaries;

public record DictionaryItemDetailDto : DictionaryItemDto
{
    public string ConcurrencyStamp { get; set; }
}