namespace Dddify.Admin.Domain.Exceptions.Roles;

public class RoleNameDuplicateException : DomainException
{
    public override string Name => "role_name_duplicate";

    public RoleNameDuplicateException(string name)
        : base($"Role with name '{name}' already exists.")
    {
    }
}