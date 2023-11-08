using Dddify.Admin.Application.Dtos.Tokens;
using Dddify.Admin.Application.Exceptions.Tokens;

namespace Dddify.Admin.Application.Commands.Tokens;

public record GenerateTokenByPhoneCommand(
    string PhoneNumber, 
    string Captcha) : ICommand<TokenDto>;

public class GenerateTokenByPhoneCommandValidator : AbstractValidator<GenerateTokenByPhoneCommand>
{
    public GenerateTokenByPhoneCommandValidator()
    {
        RuleFor(v => v.PhoneNumber).NotEmpty().Matches("^1[0-9]{10}$");
        RuleFor(v => v.Captcha).NotEmpty().Matches("^[0-9]+$");
    }
}

public class GenerateTokenByPhoneCommandHandler : ICommandHandler<GenerateTokenByPhoneCommand, TokenDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtHelper _jwtHelper;

    public GenerateTokenByPhoneCommandHandler(IApplicationDbContext context, IJwtHelper jwtHelper, IPasswordHelper passwordHelper)
    {
        _context = context;
        _jwtHelper = jwtHelper;
    }

    public async Task<TokenDto> Handle(GenerateTokenByPhoneCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(c => c.RefreshTokens)
            .FirstOrDefaultAsync(c => c.PhoneNumber == command.PhoneNumber, cancellationToken);

        if (user is null)
        {
            throw new InvalidPhoneNumberException(command.PhoneNumber);
        }

        if (command.Captcha != "666666")
        {
            throw new CaptchaErrorException(command.PhoneNumber, command.Captcha);
        }

        var (accessToken, _) = _jwtHelper.GenerateAccessToken(user.JwtClaims());
        var (refreshToken, refreshTokenExpiredAt) = _jwtHelper.GenerateRefreshToken();

        user.AddRefreshToken(refreshToken, refreshTokenExpiredAt);

        await _context.SaveChangesAsync(cancellationToken);

        return new(accessToken, refreshToken);
    }
}