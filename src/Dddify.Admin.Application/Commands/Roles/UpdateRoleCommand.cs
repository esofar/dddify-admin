using Dddify.Admin.Application.Exceptions.Roles;
using Dddify.Admin.Domain.Aggregates.Roles;

namespace Dddify.Admin.Application.Commands.Roles;

public record UpdateRoleCommand(
    Guid Id,
    string Name,
    bool IsDefault,
    int Order,
    string? Description,
    string? ConcurrencyStamp) : ICommand;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Role.MaxNameLength);

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Description)
            .MaximumLength(Role.MaxDescriptionLength);
    }
}

public class UpdateRoleCommandHandler(IRoleRepository roleRepository) : IRequestHandler<UpdateRoleCommand>
{
    public async Task Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new RoleNotFoundException(command.Id);

        if (!await roleRepository.IsNameUniqueAsync(command.Name, role.Id))
        {
            throw new RoleNameDuplicateException(command.Name);
        }

        role.Change(
            command.Name,
            command.IsDefault,
            command.Order,
            command.Description);

        roleRepository.SetOriginalConcurrencyStamp(role, command.ConcurrencyStamp);
    }
}
