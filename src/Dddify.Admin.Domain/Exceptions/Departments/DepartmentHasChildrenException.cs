namespace Dddify.Admin.Domain.Exceptions.Departments;

public class DepartmentHasChildrenException : DomainException
{
    public DepartmentHasChildrenException(Guid id)
    {
        WithErrorCode("department_has_children");
        WithMetadata("DepartmentId", id);
    }
}
