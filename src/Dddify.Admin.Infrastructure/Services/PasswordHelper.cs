using Dddify.Admin.Application.Interfaces;

namespace Dddify.Admin.Infrastructure.Services;

public class PasswordHelper : IPasswordHelper
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}