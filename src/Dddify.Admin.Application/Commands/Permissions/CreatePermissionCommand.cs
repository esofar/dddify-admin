using Dddify.Admin.Application.Exceptions.Permissions;
using Dddify.Admin.Domain.Aggregates.Permissions;

namespace Dddify.Admin.Application.Commands.Permissions;

public record CreatePermissionCommand(
    Guid? ParentId,
    string Code,
    string Name,
    string Type,
    int Order) : ICommand;

public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(Permission.MaxCodeLength);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Permission.MaxNameLength);

        RuleFor(x => x.Type)
            .NotEmpty()
            .MaximumLength(20)
            .IsEnumName(typeof(PermissionType), caseSensitive: false);

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0);
    }
}

public class CreatePermissionCommandHandler(IPermissionRepository permissionRepository, IGuidGenerator guidGenerator) : ICommandHandler<CreatePermissionCommand>
{
    public async Task Handle(CreatePermissionCommand command, CancellationToken cancellationToken)
    {
        if (!await permissionRepository.IsCodeUniqueAsync(command.Code))
        {
            throw new PermissionCodeAlreadyExistsException(command.Code);
        }

        if (!await permissionRepository.IsNameUniqueAsync(command.Name, command.ParentId))
        {
            throw new PermissionNameAlreadyExistsException(command.Name);
        }

        var type = Enum.Parse<PermissionType>(command.Type.Trim(), ignoreCase: true);

        var permission = new Permission(
            guidGenerator.Create(),
            command.ParentId,
            command.Code,
            command.Name,
            type,
            command.Order
        );

        await permissionRepository.AddAsync(permission, cancellationToken);
    }
}
