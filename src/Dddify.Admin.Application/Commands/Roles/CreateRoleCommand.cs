using Dddify.Admin.Application.Exceptions.Roles;
using Dddify.Admin.Domain.Aggregates.Roles;

namespace Dddify.Admin.Application.Commands.Roles;

public record CreateRoleCommand(string Name, bool IsDefault, int Order, string Description) : ICommand;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Role.MaxNameLength);

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(Role.MaxDescriptionLength);
    }
}

public class CreateRoleCommandHandler(IRoleRepository roleRepository, IGuidGenerator guidGenerator) : ICommandHandler<CreateRoleCommand>
{
    public async Task Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        if (!await roleRepository.IsNameUniqueAsync(command.Name))
        {
            throw new RoleNameDuplicateException(command.Name);
        }

        var role = new Role(
            guidGenerator.Create(),
            command.Name,
            command.IsDefault,
            command.Order,
            command.Description
        );

        await roleRepository.AddAsync(role, cancellationToken);
    }
}
