using Dddify.Admin.Application.Dtos.Tokens;
using Dddify.Admin.Application.Exceptions.Tokens;
using Dddify.Admin.Domain.Exceptions.Users;

namespace Dddify.Admin.Application.Commands.Tokens;

public record ExchangeTokenCommand(
    string AccessToken, 
    string RefreshToken) : ICommand<TokenDto>;

public class ExchangeTokenCommandValidator : AbstractValidator<ExchangeTokenCommand>
{
    public ExchangeTokenCommandValidator()
    {
        RuleFor(v => v.AccessToken).NotEmpty();
        RuleFor(v => v.RefreshToken).NotEmpty();
    }
}

public class ExchangeTokenCommandHandler : ICommandHandler<ExchangeTokenCommand, TokenDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IClock _clock;
    private readonly IJwtHelper _jwtHelper;

    public ExchangeTokenCommandHandler(IApplicationDbContext context, IClock clock, IJwtHelper jwtHelper)
    {
        _context = context;
        _clock = clock;
        _jwtHelper = jwtHelper;
    }

    public async Task<TokenDto> Handle(ExchangeTokenCommand command, CancellationToken cancellationToken)
    {
        var principal = _jwtHelper.GetPrincipalFromToken(command.AccessToken);

        var userId = principal?.Claims.FirstOrDefault(c => c.Type == DefaultClaimTypes.UserId)?.Value.ToGuid() ?? Guid.Empty;

        var user = await _context.Users
            .Include(c => c.RefreshTokens.Where(c => c.Token == command.RefreshToken))
            .FirstOrDefaultAsync(c => c.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }

        var userRefreshToken = user.RefreshTokens.FirstOrDefault();

        if (userRefreshToken is null || userRefreshToken.ExpiredAt < _clock.Now)
        {
            throw new InvalidRefreshTokenException();
        }

        var (accessToken, accessTokenExpiredAt) = _jwtHelper.GenerateAccessToken(user.JwtClaims());
        var (refreshToken, refreshTokenExpiredAt) = _jwtHelper.GenerateRefreshToken();

        user.AddRefreshToken(refreshToken, refreshTokenExpiredAt);

        await _context.SaveChangesAsync(cancellationToken);

        return new(accessToken, refreshToken);
    }
}