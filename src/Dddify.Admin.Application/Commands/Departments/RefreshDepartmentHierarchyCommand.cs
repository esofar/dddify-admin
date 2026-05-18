namespace Dddify.Admin.Application.Commands.Departments;

public record RefreshDepartmentHierarchyCommand(
    Guid DepartmentId,
    string OldPath,
    string NewPath,
    string OldFullName,
    string NewFullName) : ICommand;

public class RefreshDepartmentHierarchyCommandValidator : AbstractValidator<RefreshDepartmentHierarchyCommand>
{
    public RefreshDepartmentHierarchyCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty();

        RuleFor(x => x.OldPath)
            .NotEmpty();

        RuleFor(x => x.NewPath)
            .NotEmpty();

        RuleFor(x => x.OldFullName)
            .NotEmpty();

        RuleFor(x => x.NewFullName)
            .NotEmpty();
    }
}

public class RefreshDepartmentHierarchyCommandHandler(IDepartmentRepository departmentRepository) : ICommandHandler<RefreshDepartmentHierarchyCommand>
{
    public async Task Handle(RefreshDepartmentHierarchyCommand command, CancellationToken cancellationToken)
    {
        var descendants = await departmentRepository.GetDescendantsAsync(command.DepartmentId, cancellationToken);

        if (descendants.Count == 0)
        {
            return;
        }

        var oldPath = command.OldPath;
        var newPath = command.NewPath;
        var oldFullName = command.OldFullName;
        var newFullName = command.NewFullName;

        var deltaLevel = GetLevelFromPath(newPath) - GetLevelFromPath(oldPath);

        var updated = 0;

        foreach (var descendant in descendants)
        {
            var refreshedPath = ReplacePrefix(descendant.Path, oldPath, newPath);
            var refreshedFullName = ReplaceFullNamePrefix(descendant.FullName, oldFullName, newFullName);
            var refreshedLevel = descendant.Level + deltaLevel;

            descendant.ApplyHierarchyRefresh(refreshedPath, refreshedLevel, refreshedFullName);

            updated++;
        }
    }

    private static int GetLevelFromPath(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length - 1;
    }

    private static string ReplacePrefix(string value, string oldPrefix, string newPrefix)
    {
        return value.StartsWith(oldPrefix, StringComparison.Ordinal)
            ? string.Concat(newPrefix, value.AsSpan(oldPrefix.Length))
            : value;
    }

    private static string ReplaceFullNamePrefix(string value, string oldFullName, string newFullName)
    {
        if (!value.StartsWith(oldFullName, StringComparison.Ordinal))
            return value;

        if (value.Length == oldFullName.Length)
            return newFullName;

        if (value[oldFullName.Length] != '/')
            return value;

        return string.Concat(newFullName, value.AsSpan(oldFullName.Length));
    }
}
