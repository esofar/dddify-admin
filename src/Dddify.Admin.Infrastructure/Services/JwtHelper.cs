using Dddify.Timing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Dddify.Admin.Infrastructure.Services;

public class JwtHelper : IJwtHelper
{
    private readonly IClock _clock;
    private readonly JwtBearerOptions _options;

    public JwtHelper(IOptions<JwtBearerOptions> options, IClock clock)
    {
        _clock = clock;
        _options = options.Value;
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token, TokenValidationParameters? tokenValidationParameters = null)
    {
        tokenValidationParameters ??= new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _options.Issuer,
            ValidAudience = _options.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret!))
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

        if ((validatedToken is JwtSecurityToken jwtSecurityToken)
            && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return claimsPrincipal;
        }

        return default;
    }

    public (string accessToken, DateTime expiredAt) GenerateAccessToken(IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(_options.Secret);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: _clock.Now,
            expires: _clock.Now.AddMinutes(_options.AccessTokenExpires),
            signingCredentials: signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return new(accessToken, _clock.Now.AddMinutes(_options.AccessTokenExpires));
    }

    public (string refreshToken, DateTime expiredAt) GenerateRefreshToken()
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshTokenExpires = _clock.Now.AddMinutes(_options.RefreshTokenExpires);

        return new(refreshToken, refreshTokenExpires);
    }
}

public class JwtBearerOptions
{
    public const string SectionName = "Authentication:JwtBearer";

    public string? Issuer { get; set; }

    public string? Audience { get; set; }

    public string? Secret { get; set; }

    public int AccessTokenExpires { get; set; } = 30;

    public int RefreshTokenExpires { get; set; } = 60 * 24 * 7;
}