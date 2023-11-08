using Dddify.Admin.Domain.Exceptions.Roles;

namespace Dddify.Admin.Application.Commands.Roles;

public record DeleteRoleCommand(Guid Id) : ICommand;

public class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (role is null)
        {
            throw new RoleNotFoundException(command.Id);
        }

        _context.Roles.Remove(role);

        await _context.SaveChangesAsync(cancellationToken);
    }
}