namespace Dddify.Admin.Application.Exceptions.Departments;

public class DepartmentNotFoundException : AppException
{
    public DepartmentNotFoundException(Guid id)
    {
        WithErrorCode("department_not_found");
        WithMetadata("DepartmentId", id);
    }
}
