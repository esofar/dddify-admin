using Dddify.Admin.Application.Exceptions.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record EnableLookupItemCommand(Guid LookupId, Guid ItemId) : ICommand;

public class EnableLookupItemCommandValidator : AbstractValidator<EnableLookupItemCommand>
{
    public EnableLookupItemCommandValidator()
    {
        RuleFor(x => x.LookupId)
            .NotEmpty();

        RuleFor(x => x.ItemId)
            .NotEmpty();
    }
}

public class EnableLookupItemCommandHandler(ILookupRepository lookupRepository) : ICommandHandler<EnableLookupItemCommand>
{
    public async Task Handle(EnableLookupItemCommand command, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository.GetLookupWithItemsAsync(command.LookupId, cancellationToken)
            ?? throw new LookupNotFoundException(command.LookupId);

        lookup.EnableItem(command.ItemId);
    }
}
