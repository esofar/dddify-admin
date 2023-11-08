namespace Dddify.Admin.Domain.Exceptions.Dictionaries;

public class DictionaryItemNameDuplicateException : DomainException
{
    public override string Name => "dictionary_item_name_duplicate";

    public DictionaryItemNameDuplicateException(string name)
        : base($"Dictionary item with name '{name}' already exists.")
    {
    }
}