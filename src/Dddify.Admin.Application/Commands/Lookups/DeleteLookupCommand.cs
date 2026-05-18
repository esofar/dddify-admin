using Dddify.Admin.Application.Exceptions.Lookups;
using Dddify.Admin.Domain.Exceptions.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record DeleteLookupCommand(Guid Id) : ICommand;

public class DeleteLookupCommandHandler(ILookupRepository lookupRepository) : ICommandHandler<DeleteLookupCommand>
{
    public async Task Handle(DeleteLookupCommand command, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository.GetLookupWithItemsAsync(command.Id, cancellationToken)
            ?? throw new LookupNotFoundException(command.Id);

        if (lookup.Items.Count > 0)
        {
            throw new LookupHasItemsException(command.Id);
        }

        lookupRepository.Remove(lookup);
    }
}