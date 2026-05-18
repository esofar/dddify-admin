using Dddify.Admin.Application.Exceptions.Roles;
using Dddify.Admin.Domain.Aggregates.Roles;

namespace Dddify.Admin.Application.Commands.Roles;

public record AssignPermissionsCommand(Guid Id, IEnumerable<PermissionEntry> Permissions) : ICommand;

public record PermissionEntry(Guid PermissionId, string PermissionCode);

public class AssignPermissionsCommandValidator : AbstractValidator<AssignPermissionsCommand>
{
    public AssignPermissionsCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Permissions)
            .NotEmpty();

        RuleForEach(x => x.Permissions)
            .NotNull()
            .ChildRules(permission =>
            {
                permission.RuleFor(x => x.PermissionId)
                    .NotEmpty();

                permission.RuleFor(x => x.PermissionCode)
                    .NotEmpty()
                    .MaximumLength(RolePermission.MaxPermissionCodeLength);
            });
    }
}

public class AssignPermissionsCommandHandler(IRoleRepository roleRepository, IDistributedCache distributedCache) : IRequestHandler<AssignPermissionsCommand>
{
    public async Task Handle(AssignPermissionsCommand command, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetRoleWithPermissionsAsync(command.Id, cancellationToken)
            ?? throw new RoleNotFoundException(command.Id);

        var rolePermissions = command.Permissions.Select(p => new RolePermission(role.Id, p.PermissionId, p.PermissionCode));

        role.AssignPermissions(rolePermissions);

        await distributedCache.RemoveAsync(CacheKeys.Role.Permissions(role.Id), cancellationToken);
    }
}
