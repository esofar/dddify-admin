using Dddify.Admin.Application.Exceptions.Sms;
using Dddify.Admin.Application.Services.Sms;
using Dddify.Admin.Domain.Aggregates.Users;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Dddify.Admin.Application.Commands.Auths;

public record SendLoginSmsCodeCommand(string PhoneNumber, string? IpAddress) : ICommand;

public class SendLoginSmsCodeCommandValidator : AbstractValidator<SendLoginSmsCodeCommand>
{
    public SendLoginSmsCodeCommandValidator()
    {
        RuleFor(c => c.PhoneNumber)
            .NotEmpty()
            .MaximumLength(User.MaxPhoneNumberLength);
    }
}

public class SendLoginSmsCodeCommandHandler(
    ISmsVerificationCodeService smsVerificationCodeService,
    ISmsRateLimiter smsRateLimiter,
    ISmsSender smsSender,
    IGuidGenerator guidGenerator,
    IOptions<SmsOptions> options) : ICommandHandler<SendLoginSmsCodeCommand>
{
    public async Task Handle(SendLoginSmsCodeCommand command, CancellationToken cancellationToken)
    {
        var smsOptions = options.Value;
        var code = smsVerificationCodeService.GenerateCode();

        var requestId = guidGenerator.Create().ToString("N");
        var traceId = Activity.Current?.TraceId.ToString() ?? requestId;

        var rateLimitResult = await smsRateLimiter.CheckAsync(
            new SmsRateLimitContext(SmsScene.Login, command.PhoneNumber, command.IpAddress),
            cancellationToken);

        if (!rateLimitResult.Allowed)
        {
            throw new SmsRateLimitExceededException(rateLimitResult.Policy!, rateLimitResult.RetryAfter!.Value);
        }

        await smsVerificationCodeService.StoreAsync(
            SmsScene.Login,
            command.PhoneNumber,
            code,
            TimeSpan.FromMinutes(smsOptions.Verification.ExpiresMinutes),
            TimeSpan.FromMinutes(smsOptions.Verification.VerifyWindowMinutes),
            cancellationToken);

        var result = await smsSender.SendAsync(
            new SmsMessage(
                SmsScene.Login,
                command.PhoneNumber,
                smsOptions.Verification.LoginTemplateName,
                new Dictionary<string, string>
                {
                    ["Code"] = code,
                    ["Minutes"] = smsOptions.Verification.ExpiresMinutes.ToString()
                },
                requestId,
                traceId),
            cancellationToken);

        if (!result.Succeeded)
        {
            throw new SmsSendFailedException(result.Provider ?? "Unknown", result.ErrorCode, result.ErrorMessage);
        }
    }
}
