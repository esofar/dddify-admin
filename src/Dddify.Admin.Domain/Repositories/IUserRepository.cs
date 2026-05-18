using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Domain.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<User?> GetUserWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
    Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber, Guid? excludeUserId = null);
}
