using Dddify.Admin.Application.Dtos.Auths;
using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Application.Services.Email;
using Dddify.Admin.Domain.Aggregates.Sessions;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Auths;

public record VerificationCodeLoginCommand(
    string Email,
    string Code,
    string DeviceId,
    string DeviceName,
    string? IpAddress,
    string? UserAgent,
    bool RememberMe) : ICommand<TokenDto>;

public class VerificationCodeLoginCommandValidator : AbstractValidator<VerificationCodeLoginCommand>
{
    public VerificationCodeLoginCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().MaximumLength(User.MaxEmailLength).EmailAddress();
        RuleFor(c => c.Code).NotEmpty().Matches("^[0-9]{6}$");
        RuleFor(c => c.DeviceId).NotEmpty().MaximumLength(Session.MaxDeviceIdLength);
        RuleFor(c => c.DeviceName).NotEmpty().MaximumLength(Session.MaxDeviceNameLength);
        RuleFor(c => c.IpAddress).MaximumLength(Session.MaxIpAddressLength);
        RuleFor(c => c.UserAgent).MaximumLength(Session.MaxUserAgentLength);
    }
}

public class VerificationCodeLoginCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationCodeService codeService,
    ITokenService tokenService,
    IClock clock) : ICommandHandler<VerificationCodeLoginCommand, TokenDto>
{
    public async Task<TokenDto> Handle(VerificationCodeLoginCommand command, CancellationToken cancellationToken)
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
        var accessToken = tokenService.GenerateAccessToken(user.Id);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.MarkLoginSucceeded(
            command.DeviceId,
            command.DeviceName,
            command.IpAddress,
            command.UserAgent,
            tokenService.HashRefreshToken(refreshToken),
            tokenService.GetRefreshTokenExpiresAt(now),
            now,
            command.RememberMe);

        return new TokenDto(accessToken, refreshToken, command.RememberMe);
    }
}
