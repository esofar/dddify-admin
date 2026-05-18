using Dddify.Admin.Application.Dtos.Auths;
using Dddify.Admin.Application.Exceptions;
using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Auths;

public record AccountLoginCommand(
    string Account,
    string Password,
    string DeviceId,
    string DeviceName,
    string? IpAddress,
    string? UserAgent,
    bool RememberMe) : ICommand<TokenDto>;

public class AccountLoginCommandValidator : AbstractValidator<AccountLoginCommand>
{
    public AccountLoginCommandValidator()
    {
        RuleFor(v => v.Account)
            .NotEmpty()
            .MaximumLength(User.MaxEmailLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(User.MaxPasswordLength);

        RuleFor(v => v.DeviceId)
            .NotEmpty();

        RuleFor(v => v.DeviceName)
            .NotEmpty();
    }
}

public class AccountLoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenHelper,
    IClock clock) : ICommandHandler<AccountLoginCommand, TokenDto>
{
    public async Task<TokenDto> Handle(AccountLoginCommand command, CancellationToken cancellationToken)
    {
        var user = command.Account.Contains('@')
            ? await userRepository.GetByEmailAsync(command.Account)
            : await userRepository.GetByPhoneNumberAsync(command.Account);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new UserInvalidCredentialsException(command.Account);
        }

        user.EnsureActive();

        var now = clock.UtcNow;
        var accessToken = tokenHelper.GenerateAccessToken(user.Id);
        var refreshToken = tokenHelper.GenerateRefreshToken();
        var refreshTokenHash = tokenHelper.HashRefreshToken(refreshToken);
        var expiresAt = tokenHelper.GetRefreshTokenExpiresAt(now);

        user.MarkLoginSucceeded(
            command.DeviceId,
            command.DeviceName,
            command.IpAddress,
            command.UserAgent,
            refreshTokenHash,
            expiresAt,
            now,
            command.RememberMe);

        return new TokenDto(accessToken, refreshToken, command.RememberMe);
    }
}
