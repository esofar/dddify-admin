using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record AssignUserRolesCommand(Guid UserId, IEnumerable<RoleEntry> Roles) : ICommand;

public record RoleEntry(Guid RoleId, string RoleName);

public class AssignUserRolesCommandValidator : AbstractValidator<AssignUserRolesCommand>
{
    public AssignUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Roles)
            .NotEmpty();

        RuleForEach(x => x.Roles)
            .NotNull()
            .ChildRules(role =>
            {
                role.RuleFor(x => x.RoleId)
                    .NotEmpty();

                role.RuleFor(x => x.RoleName)
                    .NotEmpty()
                    .MaximumLength(UserRole.MaxRoleNameLength);
            });
    }
}

public class AssignUserRolesCommandHandler(IUserRepository userRepository, IDistributedCache distributedCache) : ICommandHandler<AssignUserRolesCommand>
{
    public async Task Handle(AssignUserRolesCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithRolesAsync(command.UserId, cancellationToken)
            ?? throw new UserNotFoundException(command.UserId);

        var userRoles = command.Roles.Select(p => new UserRole(user.Id, p.RoleId, p.RoleName));

        user.AssignRoles(userRoles);

        await distributedCache.RemoveAsync(CacheKeys.User.Roles(user.Id), cancellationToken);
    }
}
