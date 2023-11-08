namespace Dddify.Admin.Application.Exceptions.Tokens;

public class InvalidRefreshTokenException : AppException
{
    public override string Name => "invalid_refresh_token";

    public InvalidRefreshTokenException()
        : base($"Invalid refresh token.")
    {
    }
}