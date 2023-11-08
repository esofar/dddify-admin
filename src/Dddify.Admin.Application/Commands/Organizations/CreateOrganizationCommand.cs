using Dddify.Admin.Domain.Exceptions.Organizations;

namespace Dddify.Admin.Application.Commands.Organizations;

public record CreateOrganizationCommand(
    Guid? ParentId,
    string Name,
    string Type,
    int Order,
    bool IsEnabled) : ICommand;

public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Type).NotEmpty();
    }
}

public class CreateOrganizationCommandHandler : ICommandHandler<CreateOrganizationCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateOrganizationCommand command, CancellationToken cancellationToken)
    {
        if (await _context.Organizations.AnyAsync(c => c.Name == command.Name, cancellationToken))
        {
            throw new OrganizationNameDuplicateException(command.Name);
        }

        var organization = new Organization(
            command.ParentId,
            command.Name,
            command.Type,
            command.Order,
            command.IsEnabled);

        await _context.Organizations.AddAsync(organization, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}