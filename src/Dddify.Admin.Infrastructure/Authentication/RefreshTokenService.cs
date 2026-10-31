using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;

namespace Dddify.Admin.Infrastructure.Authentication;

[ScopedDependency(RegistrationMode.AsMatchingInterface)]
public class RefreshTokenService(IOptions<JwtOptions> jwtOptions) : IRefreshTokenService
{
    private readonly JwtOptions jwt = jwtOptions.Value;

    public GeneratedRefreshToken Generate(DateTimeOffset now)
    {
        var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        return new GeneratedRefreshToken(token, Hash(token), now.AddDays(jwt.RefreshTokenDays), jwt.RefreshTokenDays);
    }

    public string Hash(string token)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
