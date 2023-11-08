namespace Dddify.Admin.Domain.Exceptions.Dictionaries;

public class DictionaryCodeDuplicateException : DomainException
{
    public override string Name => "dictionary_code_duplicate";

    public DictionaryCodeDuplicateException(string code)
        : base($"Dictionary with code '{code}' already exists.")
    {
    }
}