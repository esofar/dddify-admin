using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Dddify.Admin.Application.Interfaces;

public interface IJwtHelper
{
    ClaimsPrincipal? GetPrincipalFromToken(string token, TokenValidationParameters? tokenValidationParameters = null);

    (string accessToken, DateTime expiredAt) GenerateAccessToken(IEnumerable<Claim> claims);

    (string refreshToken, DateTime expiredAt) GenerateRefreshToken();
}