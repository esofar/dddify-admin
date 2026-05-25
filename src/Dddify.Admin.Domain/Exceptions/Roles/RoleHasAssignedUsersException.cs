namespace Dddify.Admin.Domain.Exceptions.Roles;

public class RoleHasAssignedUsersException : DomainException
{
    public RoleHasAssignedUsersException(Guid roleId, int assignedUserCount)
    {
        WithErrorCode("role_has_assigned_users", assignedUserCount);
        WithMetadata("RoleId", roleId);
    }
}
