namespace Dddify.Admin.Domain.Exceptions.Dictionaries;

public class DictionaryNameDuplicateException : DomainException
{
    public override string Name => "dictionary_name_duplicate";

    public DictionaryNameDuplicateException(string name)
        : base($"Dictionary with name '{name}' already exists.")
    {
    }
}