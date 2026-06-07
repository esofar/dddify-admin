using Dddify.Admin.Domain.Aggregates.InboxItems;

namespace Dddify.Admin.Application.Commands.InboxItems;

public record CreateInboxItemsCommand(
    IEnumerable<Guid> UserIds,
    string Title,
    string Summary,
    InboxItemSourceType SourceType,
    Guid SourceId) : ICommand;

public class CreateInboxItemsCommandValidator : AbstractValidator<CreateInboxItemsCommand>
{
    public CreateInboxItemsCommandValidator()
    {
        RuleFor(c => c.UserIds)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(InboxItem.MaxTitleLength);

        RuleFor(c => c.Summary)
            .MaximumLength(InboxItem.MaxSummaryLength);

        RuleFor(c => c.SourceType)
            .NotEmpty();

        RuleFor(c => c.SourceId)
            .NotEmpty();
    }
}

public class CreateInboxItemsCommandHandler(
    IInboxItemRepository inboxItemRepository,
    IGuidGenerator guidGenerator) : ICommandHandler<CreateInboxItemsCommand>
{
    public async Task Handle(CreateInboxItemsCommand command, CancellationToken cancellationToken)
    {
        var inboxItems = command.UserIds.Select(userId => new InboxItem(
            guidGenerator.Create(),
            userId,
            command.Title,
            command.Summary,
            new InboxItemSource(command.SourceType, command.SourceId)));

        await inboxItemRepository.AddRangeAsync(inboxItems, cancellationToken);
    }
}
