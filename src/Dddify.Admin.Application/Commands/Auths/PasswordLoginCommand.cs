using Dddify.Admin.Application.Dtos.Auths;
using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Auths;

public record PasswordLoginCommand(
    string Email,
    string Password,
    string DeviceId,
    string DeviceName,
    string? IpAddress,
    string? UserAgent,
    bool RememberMe) : ICommand<TokenDto>;

public class PasswordLoginCommandValidator : AbstractValidator<PasswordLoginCommand>
{
    public PasswordLoginCommandValidator()
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

public class PasswordLoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IClock clock) : ICommandHandler<PasswordLoginCommand, TokenDto>
{
    public async Task<TokenDto> Handle(PasswordLoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(command.Email.Trim().ToLowerInvariant());

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new UserInvalidCredentialsException(command.Email);
        }

        user.EnsureActive();

        var accessToken = accessTokenService.Generate(user.Id);
        var refreshToken = refreshTokenService.Generate();
        var refreshTokenHash = refreshTokenService.Hash(refreshToken);
        var expiresAt = refreshTokenService.GetExpiresAt(clock.UtcNow);

        user.MarkLoginSucceeded(
            command.DeviceId,
            command.DeviceName,
            command.IpAddress,
            command.UserAgent,
            refreshTokenHash,
            expiresAt,
            clock.UtcNow,
            command.RememberMe);

        return new TokenDto(accessToken, refreshToken, command.RememberMe);
    }
}
