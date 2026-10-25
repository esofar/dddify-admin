namespace Dddify.Admin.Application.Abstractions.Email;

public interface IEmailVerificationCodeService
{
    Task<string> CreateAsync(string email, string? ipAddress, CancellationToken cancellationToken);

    Task<bool> VerifyAsync(string email, string code, CancellationToken cancellationToken);

    Task RemoveAsync(string email, string code, CancellationToken cancellationToken);
}
