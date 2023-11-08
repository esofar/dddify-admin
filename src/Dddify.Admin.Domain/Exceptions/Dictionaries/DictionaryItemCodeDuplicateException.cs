namespace Dddify.Admin.Domain.Exceptions.Dictionaries;

public class DictionaryItemCodeDuplicateException : DomainException
{
    public override string Name => "dictionary_item_code_duplicate";

    public DictionaryItemCodeDuplicateException(string code)
        : base($"Dictionary item with code '{code}' already exists.")
    {
    }
}