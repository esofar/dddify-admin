namespace Dddify.Admin.Application.Dtos.Dictionaries;

public record DictionaryDetailDto : DictionaryDto
{
    public string ConcurrencyStamp { get; set; }
}