using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record ResetUserPasswordCommand(Guid UserId, string NewPassword) : ICommand;

public class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(User.MaxPasswordLength)
            .Matches(User.PasswordPattern);
    }
}

public class ResetUserPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher) : ICommandHandler<ResetUserPasswordCommand>
{
    public async Task Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(command.UserId, cancellationToken)
            ?? throw new UserNotFoundException(command.UserId);

        var newPasswordHash = passwordHasher.Hash(command.NewPassword);

        user.ResetPassword(newPasswordHash, command.NewPassword);
    }
}
