namespace Dddify.Admin.Application.Exceptions.Sessions;

public class SessionInvalidRefreshTokenException : AppException
{
    public SessionInvalidRefreshTokenException()
    {
        WithErrorCode("session_invalid_refresh_token");
    }
}
