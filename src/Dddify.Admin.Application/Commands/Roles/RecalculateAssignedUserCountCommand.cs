namespace Dddify.Admin.Application.Commands.Roles;

public record RecalculateAssignedUserCountCommand(
    IEnumerable<Guid> AddedRoleIds,
    IEnumerable<Guid> RemovedRoleIds) : ICommand;

public class RecalculateAssignedUserCountCommandValidator : AbstractValidator<RecalculateAssignedUserCountCommand>
{
    public RecalculateAssignedUserCountCommandValidator()
    {
        RuleFor(c => c.AddedRoleIds)
            .NotNull();

        RuleFor(c => c.RemovedRoleIds)
            .NotNull();

        RuleFor(c => c)
            .Must(c => c.AddedRoleIds.Any() || c.RemovedRoleIds.Any())
            .When(c => c.AddedRoleIds is not null && c.RemovedRoleIds is not null);

        RuleFor(c => c)
            .Must(c =>
            {
                var added = c.AddedRoleIds.Distinct();
                var removed = c.RemovedRoleIds.Distinct();

                return !added.Intersect(removed).Any();
            })
            .When(c => c.AddedRoleIds is not null && c.RemovedRoleIds is not null);
    }
}

public class RecalculateAssignedUserCountCommandHandler(IRoleRepository roleRepository) : ICommandHandler<RecalculateAssignedUserCountCommand>
{
    public async Task Handle(RecalculateAssignedUserCountCommand command, CancellationToken cancellationToken)
    {
        var addedSet = command.AddedRoleIds.ToHashSet();
        var removedSet = command.RemovedRoleIds.ToHashSet();

        var targetIds = addedSet.Union(removedSet).ToHashSet();

        var roles = await roleRepository.GetListAsync(r => targetIds.Contains(r.Id), cancellationToken);

        foreach (var role in roles)
        {
            if (addedSet.Contains(role.Id))
            {
                role.IncreaseAssignedUserCount();
            }

            if (removedSet.Contains(role.Id))
            {
                role.DecreaseAssignedUserCount();
            }
        }
    }
}
