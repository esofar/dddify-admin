namespace Dddify.Admin.Application.Interfaces;

public interface IPasswordHelper
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}