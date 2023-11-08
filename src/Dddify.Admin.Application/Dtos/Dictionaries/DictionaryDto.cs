namespace Dddify.Admin.Application.Dtos.Dictionaries;

public record DictionaryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}