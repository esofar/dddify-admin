namespace Dddify.Admin.Domain.Exceptions.Dictionaries;

public class DictionaryNotFoundException : DomainException
{
    public override string Name => "dictionary_not_found";

    public DictionaryNotFoundException(Guid id)
        : base($"Dictionary with id '{id}' was not found.")
    {
    }

    public DictionaryNotFoundException(string code)
        : base($"Dictionary with code '{code}' was not found.")
    {
    }
}