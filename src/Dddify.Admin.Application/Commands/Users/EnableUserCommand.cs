using Dddify.Admin.Application.Exceptions.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record EnableUserCommand(Guid UserId) : ICommand;

public class EnableUserCommandValidator : AbstractValidator<EnableUserCommand>
{
    public EnableUserCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty();
    }
}

public class EnableUserCommandHandler(IUserRepository userRepository) : ICommandHandler<EnableUserCommand>
{
    public async Task Handle(EnableUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(command.UserId, cancellationToken)
            ?? throw new UserNotFoundException(command.UserId);

        user.Enable();
    }
}
