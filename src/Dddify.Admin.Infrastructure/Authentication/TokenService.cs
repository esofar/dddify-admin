using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Dddify.Admin.Infrastructure.Authentication;

[ScopedDependency(RegistrationMode.AsMatchingInterface)]
public class TokenService(IClock clock, IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private readonly JwtOptions jwt = jwtOptions.Value;

    public string GenerateAccessToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            notBefore: clock.UtcNow.UtcDateTime,
            expires: clock.UtcNow.AddMinutes(jwt.AccessTokenMinutes).UtcDateTime,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    public string GenerateRefreshToken()
    {
        return WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
    }

    public DateTimeOffset GetRefreshTokenExpiresAt(DateTimeOffset now)
    {
        return now.AddDays(jwt.RefreshTokenDays);
    }

    public string HashRefreshToken(string refreshToken)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }
}
