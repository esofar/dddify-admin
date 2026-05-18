using Dddify.Admin.Application.Exceptions.Departments;
using Dddify.Admin.Domain.Exceptions.Departments;

namespace Dddify.Admin.Application.Commands.Departments;

public record DeleteDepartmentCommand(Guid Id) : ICommand;

public class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
{
    public DeleteDepartmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DeleteDepartmentCommandHandler(IDepartmentRepository departmentRepository) : ICommandHandler<DeleteDepartmentCommand>
{
    public async Task Handle(DeleteDepartmentCommand command, CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new DepartmentNotFoundException(command.Id);

        if (await departmentRepository.AnyAsync(d => d.ParentId == command.Id, cancellationToken))
        {
            throw new DepartmentHasChildrenException(command.Id);
        }

        departmentRepository.Remove(department);
    }
}
