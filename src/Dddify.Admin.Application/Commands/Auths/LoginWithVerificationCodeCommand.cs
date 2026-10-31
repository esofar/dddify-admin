using Dddify.Admin.Application.Dtos.Auths;
using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Application.Abstractions.Email;
using Dddify.Admin.Application.Validation;
using Dddify.Admin.Domain.Aggregates.Sessions;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Auths;

public record LoginWithVerificationCodeCommand(
    string Email,
    string Code,
    string DeviceId,
    string DeviceName,
    string? IpAddress,
    string? UserAgent,
    bool IsPersistent) : ICommand<TokenPairDto>;

public class LoginWithVerificationCodeCommandValidator : AbstractValidator<LoginWithVerificationCodeCommand>
{
    public LoginWithVerificationCodeCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .MaximumLength(User.MaxEmailLength)
            .EmailAddress();

        RuleFor(c => c.Code)
            .NotEmpty()
            .VerificationCode();

        RuleFor(c => c.DeviceId)
            .NotEmpty()
            .MaximumLength(Session.MaxDeviceIdLength);

        RuleFor(c => c.DeviceName)
            .NotEmpty()
            .MaximumLength(Session.MaxDeviceNameLength);

        RuleFor(c => c.IpAddress)
            .MaximumLength(Session.MaxIpAddressLength);

        RuleFor(c => c.UserAgent)
            .MaximumLength(Session.MaxUserAgentLength);
    }
}

public class LoginWithVerificationCodeCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationCodeService codeService,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IClock clock) : ICommandHandler<LoginWithVerificationCodeCommand, TokenPairDto>
{
    public async Task<TokenPairDto> Handle(LoginWithVerificationCodeCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        if (!await codeService.VerifyAsync(email, command.Code, cancellationToken))
        {
            throw new UserInvalidEmailVerificationCodeException();
        }

        var user = await userRepository.GetByEmailAsync(email)
            ?? throw new UserInvalidEmailVerificationCodeException();
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
