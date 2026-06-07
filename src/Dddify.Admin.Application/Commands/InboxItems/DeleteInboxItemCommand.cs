using Dddify.Admin.Application.Exceptions.InboxItems;

namespace Dddify.Admin.Application.Commands.InboxItems;

public record DeleteInboxItemCommand(Guid Id, Guid UserId) : ICommand;

public class DeleteInboxItemCommandValidator : AbstractValidator<DeleteInboxItemCommand>
{
    public DeleteInboxItemCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();

        RuleFor(c => c.UserId)
            .NotEmpty();
    }
}

public class DeleteInboxItemCommandHandler(IInboxItemRepository inboxItemRepository) : ICommandHandler<DeleteInboxItemCommand>
{
    public async Task Handle(DeleteInboxItemCommand command, CancellationToken cancellationToken)
    {
        var inboxItem = await inboxItemRepository.GetUserInboxItemAsync(command.Id, command.UserId, cancellationToken)
            ?? throw new InboxItemNotFoundException(command.Id);

        inboxItem.EnsureDelete();

        inboxItemRepository.Remove(inboxItem);
    }
}
