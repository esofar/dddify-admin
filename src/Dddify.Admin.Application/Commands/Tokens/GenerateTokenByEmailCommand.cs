using Dddify.Admin.Application.Dtos.Tokens;
using Dddify.Admin.Application.Exceptions.Tokens;

namespace Dddify.Admin.Application.Commands.Tokens;

public record GenerateTokenByEmailCommand(
    string Email,
    string Password) : ICommand<TokenDto>;

public class GenerateTokenByEmailCommandValidator : AbstractValidator<GenerateTokenByEmailCommand>
{
    public GenerateTokenByEmailCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.Password).NotEmpty();
    }
}

public class GenerateTokenByEmailCommandHandler : ICommandHandler<GenerateTokenByEmailCommand, TokenDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtHelper _jwtHelper;
    private readonly IPasswordHelper _passwordHelper;

    public GenerateTokenByEmailCommandHandler(IApplicationDbContext context, IJwtHelper jwtHelper, IPasswordHelper passwordHelper)
    {
        _context = context;
        _jwtHelper = jwtHelper;
        _passwordHelper = passwordHelper;
    }

    public async Task<TokenDto> Handle(GenerateTokenByEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(c => c.RefreshTokens)
            .FirstOrDefaultAsync(c => c.Email == command.Email, cancellationToken);

        if (user is null || !_passwordHelper.Verify(command.Password, user.PasswordHash))
        {
            throw new CredentialsErrorException(command.Email);
        }

        var (accessToken, _) = _jwtHelper.GenerateAccessToken(user.JwtClaims());
        var (refreshToken, refreshTokenExpiredAt) = _jwtHelper.GenerateRefreshToken();

        user.AddRefreshToken(refreshToken, refreshTokenExpiredAt);

        await _context.SaveChangesAsync(cancellationToken);

        return new(accessToken, refreshToken);
    }
}