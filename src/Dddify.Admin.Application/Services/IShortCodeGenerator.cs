namespace Dddify.Admin.Application.Services;

public interface IShortCodeGenerator
{
    string Generate(Guid id, int length = 8);
}