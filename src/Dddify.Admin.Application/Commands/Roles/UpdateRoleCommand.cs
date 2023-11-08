using Dddify.Admin.Domain.Exceptions.Dictionaries;
using Dddify.Admin.Domain.Exceptions.Roles;

namespace Dddify.Admin.Application.Commands.Roles;

public record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description,
    string ConcurrencyStamp) : ICommand;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Description).MaximumLength(50);
        RuleFor(c => c.ConcurrencyStamp).NotEmpty();
    }
}

public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (role is null)
        {
            throw new RoleNotFoundException(command.Id);
        }

        if (await _context.Roles.AnyAsync(c => c.Id != command.Id && c.Name == command.Name, cancellationToken))
        {
            throw new DictionaryNameDuplicateException(command.Name);
        }

        _context.ResetConcurrencyStamp(role, command.ConcurrencyStamp);

        role.Update(command.Name, command.Description);

        await _context.SaveChangesAsync(cancellationToken);
    }
}