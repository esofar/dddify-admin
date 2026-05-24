using Dddify.Admin.Application.Exceptions.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record ResetUserPasswordCommand(Guid UserId) : ICommand;

public class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty();
    }
}

public class ResetUserPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IPasswordGenerator passwordGenerator) : ICommandHandler<ResetUserPasswordCommand>
{
    public async Task Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(command.UserId, cancellationToken)
            ?? throw new UserNotFoundException(command.UserId);

        var newPassword = passwordGenerator.Generate();
        var newPasswordHash = passwordHasher.Hash(newPassword);

        user.ResetPassword(newPasswordHash, newPassword);
    }
}
