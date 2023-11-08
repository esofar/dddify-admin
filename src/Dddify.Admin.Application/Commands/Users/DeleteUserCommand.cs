using Dddify.Admin.Domain.Exceptions.Users;
using Microsoft.EntityFrameworkCore;

namespace Dddify.Admin.Application.Commands.Users;

public record DeleteUserCommand(Guid Id) : ICommand;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException(command.Id);
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync(cancellationToken);
    }
}