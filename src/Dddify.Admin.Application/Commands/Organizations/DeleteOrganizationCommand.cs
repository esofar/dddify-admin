using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Commands.Organizations;

public record DeleteOrganizationCommand(Guid Id) : ICommand;

public class DeleteOrganizationCommandValidator : AbstractValidator<DeleteOrganizationCommand>
{
    public DeleteOrganizationCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class DeleteOrganizationCommandHandler : ICommandHandler<DeleteOrganizationCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteOrganizationCommand command, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (organization is null)
        {
            throw new OrganizationNotFoundException(command.Id);
        }

        if (!organization.CanBeDeleted)
        {
            throw new OrganizationCannotDeleteException(command.Id);
        }

        _context.Organizations.Remove(organization);

        await _context.SaveChangesAsync(cancellationToken);
    }
}