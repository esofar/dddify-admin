namespace Dddify.Admin.Domain.Exceptions.Departments;

public class DepartmentNameAlreadyExistsException : DomainException
{
    public DepartmentNameAlreadyExistsException(string name)
    {
        WithErrorCode("department_name_already_exists");
        WithMetadata("Name", name);
    }
}
