namespace Dddify.Admin.Application.Abstractions.Identifiers;

public interface IShortCodeGenerator
{
    string Generate(Guid id, int length = 8);
}