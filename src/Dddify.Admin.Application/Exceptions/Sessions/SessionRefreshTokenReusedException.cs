namespace Dddify.Admin.Application.Exceptions.Sessions;

public class SessionRefreshTokenReusedException : AppException
{
    public SessionRefreshTokenReusedException(Guid userId)
    {
        WithErrorCode("session_refresh_token_reused");
        WithMetadata("UserId", userId);
    }
}
