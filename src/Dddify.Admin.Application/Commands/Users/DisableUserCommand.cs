using Dddify.Admin.Application.Exceptions.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record DisableUserCommand(Guid UserId) : ICommand;

public class DisableUserCommandValidator : AbstractValidator<DisableUserCommand>
{
    public DisableUserCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty();
    }
}

public class DisableUserCommandHandler(IUserRepository userRepository) : ICommandHandler<DisableUserCommand>
{
    public async Task Handle(DisableUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(command.UserId, cancellationToken)
            ?? throw new UserNotFoundException(command.UserId);

        user.Disable();
    }
}
