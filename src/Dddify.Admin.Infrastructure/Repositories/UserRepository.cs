using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<ApplicationDbContext, User, Guid>, IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(user => user.Roles)
            .Where(user => user.Roles.Any(ur => ur.RoleId == roleId))
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetUserWithRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(user => user.Roles)
            .FirstAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null)
    {
        return !await _context.Users
            .AsNoTracking()
            .WhereIf(excludeUserId != null, c => c.Id != excludeUserId)
            .AnyAsync(user => user.Email == email);
    }

    public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber, Guid? excludeUserId = null)
    {
        return !await _context.Users
            .AsNoTracking()
            .WhereIf(excludeUserId != null, c => c.Id != excludeUserId)
            .AnyAsync(user => user.PhoneNumber == phoneNumber);
    }
}
