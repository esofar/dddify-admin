namespace Dddify.Admin.Application.Commands.Roles;

public record RefreshRoleUserCountCommand(
    IEnumerable<Guid> AddedRoleIds,
    IEnumerable<Guid> RemovedRoleIds) : ICommand;

public class RefreshRoleUserCountCommandValidator : AbstractValidator<RefreshRoleUserCountCommand>
{
    public RefreshRoleUserCountCommandValidator()
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

public class RefreshRoleUserCountCommandHandler(IRoleRepository roleRepository) : ICommandHandler<RefreshRoleUserCountCommand>
{
    public async Task Handle(RefreshRoleUserCountCommand command, CancellationToken cancellationToken)
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
