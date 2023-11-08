namespace Dddify.Admin.Domain.Exceptions.Dictionaries;

public class DictionaryItemNotFoundException : DomainException
{
    public override string Name => "dictionary_item_not_found";

    public DictionaryItemNotFoundException(Guid id)
        : base($"Dictionary item with id '{id}' was not found.")
    {
    }
}