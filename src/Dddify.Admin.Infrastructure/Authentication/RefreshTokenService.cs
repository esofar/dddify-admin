using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;

namespace Dddify.Admin.Infrastructure.Authentication;

[ScopedDependency(RegistrationMode.AsMatchingInterface)]
public class RefreshTokenService(IOptions<JwtOptions> jwtOptions) : IRefreshTokenService
{
    private readonly JwtOptions jwt = jwtOptions.Value;

    public string Generate()
    {
        return WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
    }

    public string Hash(string token)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }

    public DateTimeOffset GetExpiresAt(DateTimeOffset now)
    {
        return now.AddDays(jwt.RefreshTokenDays);
    }
}
