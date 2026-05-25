namespace Dddify.Admin.Domain.Exceptions.Roles;

public class PresetRoleCannotBeDeletedException : DomainException
{
    public PresetRoleCannotBeDeletedException(Guid id)
    {
        WithErrorCode("role_preset_cannot_be_deleted");
        WithMetadata("RoleId", id);
    }
}
