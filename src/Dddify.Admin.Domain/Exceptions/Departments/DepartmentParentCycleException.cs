namespace Dddify.Admin.Domain.Exceptions.Departments;

public class DepartmentParentCycleException : DomainException
{
    public DepartmentParentCycleException(Guid id, Guid parentId)
    {
        WithErrorCode("department_parent_cycle");
        WithMetadata(
        [
            new("DepartmentId", id),
            new("ParentId", parentId),
        ]);
    }
}
