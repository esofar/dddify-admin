using Dddify.Admin.Application.Services.Security;

namespace Dddify.Admin.Infrastructure.Security;

[SingletonDependency(RegistrationMode.AsImplementedInterfaces)]
public class PasswordHasher : IPasswordHasher
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