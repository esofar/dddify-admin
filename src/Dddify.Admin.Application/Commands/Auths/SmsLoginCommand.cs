using Dddify.Admin.Application.Common.Sms;
using Dddify.Admin.Application.Dtos.Auths;
using Dddify.Admin.Application.Exceptions;
using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Domain.Aggregates.Users;
using Microsoft.Extensions.Options;

namespace Dddify.Admin.Application.Commands.Auths;

public record SmsLoginCommand(
    string PhoneNumber,
    string Code,
    string DeviceId,
    string DeviceName,
    string? IpAddress,
    string? UserAgent,
    bool RememberMe) : ICommand<TokenDto>;

public class SmsLoginCommandValidator : AbstractValidator<SmsLoginCommand>
{
    public SmsLoginCommandValidator()
    {
        RuleFor(c => c.PhoneNumber)
            .NotEmpty()
            .MaximumLength(User.MaxPhoneNumberLength);

        RuleFor(c => c.Code)
            .NotEmpty();

        RuleFor(c => c.DeviceId)
            .NotEmpty();

        RuleFor(c => c.DeviceName)
            .NotEmpty();
    }
}

public class SmsLoginCommandHandler(
    IUserRepository userRepository,
    ISmsVerificationCodeService verificationCodeStore,
    ITokenService tokenHelper,
    IClock clock,
    IOptions<SmsOptions> options) : ICommandHandler<SmsLoginCommand, TokenDto>
{
    public async Task<TokenDto> Handle(SmsLoginCommand command, CancellationToken cancellationToken)
    {
        var verified = await verificationCodeStore.VerifyAsync(
            SmsScene.Login,
            command.PhoneNumber,
            command.Code,
            options.Value.Verification.MaxVerifyAttempts,
            TimeSpan.FromMinutes(options.Value.Verification.VerifyWindowMinutes),
            cancellationToken);

        if (!verified)
        {
            throw new UserInvalidSmsCodeException(command.PhoneNumber);
        }

        var user = await userRepository.GetByPhoneNumberAsync(command.PhoneNumber)
            ?? throw new UserInvalidSmsCodeException(command.PhoneNumber);

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
