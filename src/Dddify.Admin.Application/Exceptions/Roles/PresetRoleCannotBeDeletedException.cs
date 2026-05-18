namespace Dddify.Admin.Application.Exceptions.Roles;

public class PresetRoleCannotBeDeletedException : AppException
{
    public PresetRoleCannotBeDeletedException(Guid id)
    {
        WithErrorCode("role_preset_cannot_be_deleted");
        WithMetadata("RoleId", id);
    }
}
