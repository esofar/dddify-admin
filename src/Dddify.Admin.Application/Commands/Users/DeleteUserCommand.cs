using Dddify.Admin.Application.Exceptions.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record DeleteUserCommand(Guid Id) : ICommand;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}

public class DeleteUserCommandHandler(IUserRepository userRepository) : ICommandHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(command.Id, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException(command.Id);
        }

        userRepository.Remove(user);
    }
}
