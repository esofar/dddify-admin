namespace Dddify.Admin.Application.Commands.InboxItems;

public record MarkInboxItemsAsReadCommand(
    IEnumerable<Guid> Ids,
    Guid UserId) : ICommand;

public class MarkInboxItemsAsReadCommandValidator : AbstractValidator<MarkInboxItemsAsReadCommand>
{
    public MarkInboxItemsAsReadCommandValidator()
    {
        RuleFor(c => c.Ids)
            .NotEmpty();

        RuleFor(c => c.UserId)
            .NotEmpty();
    }
}

public class MarkInboxItemsAsReadCommandHandler(
    IInboxItemRepository inboxItemRepository,
    IClock clock) : ICommandHandler<MarkInboxItemsAsReadCommand>
{
    public async Task Handle(MarkInboxItemsAsReadCommand command, CancellationToken cancellationToken)
    {
        var inboxItems = await inboxItemRepository.GetListAsync(c => command.Ids.Contains(c.Id) && c.UserId == command.UserId, cancellationToken);

        foreach (var inboxItem in inboxItems)
        {
            inboxItem.MarkAsRead(clock.UtcNow);
        }
    }
}
