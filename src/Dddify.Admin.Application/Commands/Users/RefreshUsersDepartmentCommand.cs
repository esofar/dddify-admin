using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record RefreshUsersDepartmentCommand(Guid DepartmentId, string DepartmentName) : ICommand;

public class RefreshUsersDepartmentCommandValidator : AbstractValidator<RefreshUsersDepartmentCommand>
{
    public RefreshUsersDepartmentCommandValidator()
    {
        RuleFor(c => c.DepartmentId)
            .NotEmpty();

        RuleFor(c => c.DepartmentName)
            .NotEmpty()
            .MaximumLength(UserDepartment.MaxNameLength);
    }
}

public class RefreshUsersDepartmentNameCommandHandler(IUserRepository userRepository) : ICommandHandler<RefreshUsersDepartmentCommand>
{
    public async Task Handle(RefreshUsersDepartmentCommand command, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetListAsync(c => c.Department.Id == command.DepartmentId, cancellationToken);

        foreach (var user in users)
        {
            user.ChangeDepartment(command.DepartmentId, command.DepartmentName);
        }
    }
}
