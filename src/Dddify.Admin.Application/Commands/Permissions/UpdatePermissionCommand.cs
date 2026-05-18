using Dddify.Admin.Application.Exceptions.Permissions;
using Dddify.Admin.Domain.Aggregates.Permissions;
using Dddify.Admin.Domain.Exceptions.Permissions;

namespace Dddify.Admin.Application.Commands.Permissions;

public record UpdatePermissionCommand(
    Guid Id,
    Guid? ParentId,
    string Code,
    string Name,
    string Type,
    int Order
) : ICommand;

public class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(Permission.MaxCodeLength);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Permission.MaxNameLength);

        RuleFor(x => x.Type)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(20)
            .IsEnumName(typeof(PermissionType), caseSensitive: false);

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0);
    }
}

public class UpdatePermissionCommandHandler(IPermissionRepository permissionRepository) : ICommandHandler<UpdatePermissionCommand>
{
    public async Task Handle(UpdatePermissionCommand command, CancellationToken cancellationToken)
    {
        var permission = await permissionRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new PermissionNotFoundException(command.Id);

        if (!await permissionRepository.IsCodeUniqueAsync(command.Code, permission.Id))
        {
            throw new PermissionCodeAlreadyExistsException(command.Code);
        }

        if (!await permissionRepository.IsNameUniqueAsync(command.Name, command.ParentId, permission.Id))
        {
            throw new PermissionNameAlreadyExistsException(command.Name);
        }

        if (command.ParentId.HasValue && await permissionRepository.IsDescendantOfAsync(command.Id, command.ParentId.Value))
        {
            throw new PermissionParentCycleException(command.Id, command.ParentId.Value);
        }

        var type = Enum.Parse<PermissionType>(command.Type.Trim(), ignoreCase: true);

        permission.Change(
            command.ParentId,
            command.Code,
            command.Name,
            type,
            command.Order);
    }
}
