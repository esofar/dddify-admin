using Dddify.Admin.Domain.Exceptions.Roles;

namespace Dddify.Admin.Application.Commands.Roles;

public record CreateRoleCommand(
    string Code,
    string Name,
    string? Description) : ICommand;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Description).MaximumLength(100);
    }
}

public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        if (await _context.Roles.AnyAsync(c => c.Code == command.Code, cancellationToken))
        {
            throw new RoleCodeDuplicateException(command.Code);
        }

        if (await _context.Roles.AnyAsync(c => c.Name == command.Name, cancellationToken))
        {
            throw new RoleNameDuplicateException(command.Name);
        }

        var role = new Role(command.Code, command.Name, command.Description);

        await _context.Roles.AddAsync(role, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}