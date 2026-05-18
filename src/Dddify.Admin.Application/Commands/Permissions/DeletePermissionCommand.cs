using Dddify.Admin.Application.Exceptions.Permissions;
using Dddify.Admin.Domain.Exceptions.Permissions;

namespace Dddify.Admin.Application.Commands.Permissions;

public record DeletePermissionCommand(Guid Id) : ICommand;

public class DeletePermissionCommandValidator : AbstractValidator<DeletePermissionCommand>
{
    public DeletePermissionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DeletePermissionCommandHandler(IPermissionRepository permissionRepository) : ICommandHandler<DeletePermissionCommand>
{
    public async Task Handle(DeletePermissionCommand command, CancellationToken cancellationToken)
    {
        var permission = await permissionRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new PermissionNotFoundException(command.Id);

        if (await permissionRepository.AnyAsync(p => p.ParentId == command.Id, cancellationToken))
        {
            throw new PermissionHasChildrenException(command.Id);
        }

        permissionRepository.Remove(permission);
    }
}
