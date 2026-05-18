using Dddify.Admin.Domain.Aggregates.Lookups;
using Dddify.Admin.Domain.Exceptions.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record CreateLookupCommand(string Code, string Name, string? Description) : ICommand;

public class CreateLookupCommandValidator : AbstractValidator<CreateLookupCommand>
{
    public CreateLookupCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(Lookup.MaxCodeLength);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Lookup.MaxNameLength);

        RuleFor(x => x.Description)
            .MaximumLength(Lookup.MaxDescriptionLength);
    }
}

public class CreateLookupCommandHandler(ILookupRepository lookupRepository, IGuidGenerator guidGenerator) : ICommandHandler<CreateLookupCommand>
{
    public async Task Handle(CreateLookupCommand command, CancellationToken cancellationToken)
    {
        var existing = await lookupRepository.AnyAsync(c => c.Code == command.Code || c.Name == command.Name, cancellationToken);

        if (existing)
        {
            throw new LookupAlreadyExistsException(command.Code, command.Name);
        }

        var lookup = new Lookup(
            guidGenerator.Create(),
            command.Code,
            command.Name,
            command.Description);

        await lookupRepository.AddAsync(lookup, cancellationToken);
    }
}
