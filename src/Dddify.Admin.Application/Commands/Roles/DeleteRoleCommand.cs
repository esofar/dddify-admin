using Dddify.Admin.Application.Exceptions.Roles;

namespace Dddify.Admin.Application.Commands.Roles;

public record DeleteRoleCommand(Guid Id) : ICommand;

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}

public class DeleteRoleCommandHandler(IRoleRepository roleRepository) : ICommandHandler<DeleteRoleCommand>
{
    public async Task Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new RoleNotFoundException(command.Id);

        role.EnsureCanDelete();

        roleRepository.Remove(role);
    }
}
