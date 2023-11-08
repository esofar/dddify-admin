using Dddify.Admin.Domain.Exceptions.Dictionaries;
using Dddify.Admin.Domain.Exceptions.Organizations;

namespace Dddify.Admin.Application.Commands.Organizations;

public record UpdateOrganizationCommand(
    Guid Id,
    Guid? ParentId,
    string Name,
    string Type,
    int Order,
    bool IsEnabled,
    string ConcurrencyStamp) : ICommand;

public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Type).NotEmpty();
        RuleFor(c => c.ParentId).Must((command, parentId) => parentId != command.Id);
        RuleFor(c => c.ConcurrencyStamp).NotEmpty();
    }
}

public class UpdateOrganizationCommandHandler : ICommandHandler<UpdateOrganizationCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateOrganizationCommand command, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (organization is null)
        {
            throw new OrganizationNotFoundException(command.Id);
        }

        if (await _context.Organizations.AnyAsync(c => c.Id != command.Id && c.Name == command.Name, cancellationToken))
        {
            throw new OrganizationNameDuplicateException(command.Name);
        }

        _context.ResetConcurrencyStamp(organization, command.ConcurrencyStamp);

        organization.Update(
            command.ParentId,
            command.Name,
            command.Type,
            command.Order,
            command.IsEnabled);

        await _context.SaveChangesAsync(cancellationToken);
    }
}