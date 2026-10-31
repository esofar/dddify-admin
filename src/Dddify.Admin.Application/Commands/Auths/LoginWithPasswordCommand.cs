using Dddify.Admin.Application.Dtos.Auths;
using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Auths;

public record LoginWithPasswordCommand(
    string Email,
    string Password,
    string DeviceId,
    string DeviceName,
    string? IpAddress,
    string? UserAgent,
    bool IsPersistent) : ICommand<TokenPairDto>;

public class LoginWithPasswordCommandValidator : AbstractValidator<LoginWithPasswordCommand>
{
    public LoginWithPasswordCommandValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty()
            .MaximumLength(User.MaxEmailLength)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(User.MaxPasswordLength);

        RuleFor(v => v.DeviceId)
            .NotEmpty();

        RuleFor(v => v.DeviceName)
            .NotEmpty();
    }
}

public class LoginWithPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IClock clock) : ICommandHandler<LoginWithPasswordCommand, TokenPairDto>
{
    public async Task<TokenPairDto> Handle(LoginWithPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(command.Email.Trim().ToLowerInvariant());

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new UserInvalidCredentialsException(command.Email);
        }

        user.EnsureActive();

        var now = clock.UtcNow;
        var accessToken = accessTokenService.Generate(user.Id);
        var refreshToken = refreshTokenService.Generate(now);

        user.MarkLoginSucceeded(
            command.DeviceId,
            command.DeviceName,
            command.IpAddress,
            command.UserAgent,
            refreshToken.Hash,
            refreshToken.ExpiresAt,
            now,
            command.IsPersistent);

        return new TokenPairDto(accessToken, refreshToken.Value, command.IsPersistent, refreshToken.RefreshTokenDays);
    }
}
