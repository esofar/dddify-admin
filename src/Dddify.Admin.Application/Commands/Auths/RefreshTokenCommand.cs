using Dddify.Admin.Application.Dtos.Auths;
using Dddify.Admin.Application.Exceptions.Sessions;
using Dddify.Admin.Domain.Aggregates.Sessions;

namespace Dddify.Admin.Application.Commands.Auths;

public record RefreshTokenCommand(
    string RefreshToken,
    string DeviceId) : ICommand<TokenDto>;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(v => v.RefreshToken)
            .NotEmpty();

        RuleFor(v => v.DeviceId)
            .NotEmpty()
            .MaximumLength(Session.MaxDeviceIdLength);
    }
}

public class RefreshTokenCommandHandler(
    ISessionRepository sessionRepository,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IClock clock) : ICommandHandler<RefreshTokenCommand, TokenDto>
{
    public async Task<TokenDto> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshTokenHash = refreshTokenService.Hash(command.RefreshToken);

        var session = await sessionRepository.GetByRefreshTokenHashAsync(refreshTokenHash, command.DeviceId, cancellationToken)
            ?? throw new SessionInvalidRefreshTokenException();

        var now = clock.UtcNow;

        if (session.MatchesPreviousToken(refreshTokenHash))
        {
            await sessionRepository.RevokeUserSessionsAsync(session.UserId, "refresh_token_reuse_detected", now, cancellationToken);
            throw new SessionRefreshTokenReusedException(session.UserId);
        }

        if (!session.MatchesCurrentToken(refreshTokenHash) || !session.IsActive(now))
        {
            throw new SessionInvalidRefreshTokenException();
        }

        var accessToken = accessTokenService.Generate(session.UserId);
        var refreshToken = refreshTokenService.Generate();
        var nextRefreshTokenHash = refreshTokenService.Hash(refreshToken);
        var expiresAt = refreshTokenService.GetExpiresAt(now);

        session.RotateRefreshToken(nextRefreshTokenHash, expiresAt, now);

        return new TokenDto(accessToken, refreshToken, session.IsPersistent);
    }
}
