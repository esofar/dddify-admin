namespace Dddify.Admin.Application.Exceptions.Roles;

public class RoleHasAssignedUsersException : AppException
{
    public RoleHasAssignedUsersException(Guid roleId, int assignedUserCount)
    {
        WithErrorCode("role_has_assigned_users", assignedUserCount);
        WithMetadata(
        [
            new("RoleId", roleId),
            new("AssignedUserCount", assignedUserCount),
        ]);
    }
}
