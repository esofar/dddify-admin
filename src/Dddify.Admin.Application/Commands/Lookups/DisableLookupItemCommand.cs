using Dddify.Admin.Application.Exceptions.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record DisableLookupItemCommand(Guid LookupId, Guid LookupItemId) : ICommand;

public class DisableLookupItemCommandValidator : AbstractValidator<DisableLookupItemCommand>
{
    public DisableLookupItemCommandValidator()
    {
        RuleFor(x => x.LookupId)
            .NotEmpty();

        RuleFor(x => x.LookupItemId)
            .NotEmpty();
    }
}

public class DisableLookupItemCommandHandler(ILookupRepository lookupRepository) : ICommandHandler<DisableLookupItemCommand>
{
    public async Task Handle(DisableLookupItemCommand command, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository.GetLookupWithItemsAsync(command.LookupId, cancellationToken)
            ?? throw new LookupNotFoundException(command.LookupId);

        lookup.DisableItem(command.LookupItemId);
    }
}
