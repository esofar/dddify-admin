namespace Dddify.Admin.Domain.Exceptions.Organizations;

public class OrganizationNameDuplicateException : DomainException
{
    public override string Name => "organization_name_duplicate";

    public OrganizationNameDuplicateException(string name)
        : base($"Organization with name '{name}' already exists.")
    {
    }
}