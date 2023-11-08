namespace Dddify.Admin.Domain.Exceptions.Dictionaries;

public class OrganizationCannotDeleteException : DomainException
{
    public override string Name => "organization_cannot_delete";

    public OrganizationCannotDeleteException(Guid id)
        : base($"Organization with id '{id}' cannot be deleted.")
    {
    }
}