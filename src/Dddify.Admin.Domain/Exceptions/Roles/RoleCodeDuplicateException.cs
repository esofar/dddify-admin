namespace Dddify.Admin.Domain.Exceptions.Roles;

public class RoleCodeDuplicateException : DomainException
{
    public override string Name => "role_code_duplicate";

    public RoleCodeDuplicateException(string code)
        : base($"Role with code '{code}' already exists.")
    {
    }
}