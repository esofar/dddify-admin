using Dddify.Admin.Application.Exceptions.Lookups;
using Dddify.Admin.Domain.Aggregates.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record UpdateLookupCommand(Guid Id, string Name, string? Description) : ICommand;

public class UpdateLookupCommandValidator : AbstractValidator<UpdateLookupCommand>
{
    public UpdateLookupCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Lookup.MaxNameLength);

        RuleFor(x => x.Description)
            .MaximumLength(Lookup.MaxDescriptionLength);
    }
}

public class UpdateLookupCommandHandler(ILookupRepository lookupRepository) : ICommandHandler<UpdateLookupCommand>
{
    public async Task Handle(UpdateLookupCommand command, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new LookupNotFoundException(command.Id);

        lookup.Change(command.Name, command.Description);
    }
}
