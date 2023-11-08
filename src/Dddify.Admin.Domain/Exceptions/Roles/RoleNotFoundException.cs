namespace Dddify.Admin.Domain.Exceptions.Roles;

public class RoleNotFoundException : DomainException
{
    public override string Name => "role_not_found";

    public RoleNotFoundException(Guid? id)
        : base($"Role with id '{id}' was not found.")
    {
    }
}