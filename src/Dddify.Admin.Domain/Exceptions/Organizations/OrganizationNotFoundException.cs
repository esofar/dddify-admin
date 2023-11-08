namespace Dddify.Admin.Domain.Exceptions.Dictionaries;

public class OrganizationNotFoundException : DomainException
{
    public override string Name => "organization_not_found";

    public OrganizationNotFoundException(Guid id)
        : base($"Organization with id '{id}' was not found.")
    {
    }
}