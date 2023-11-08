namespace Dddify.Admin.Domain.Entities.Users;

public class UserRefreshToken : FullAuditedEntity<Guid>, ISoftDeletable
{
    public UserRefreshToken(string token, DateTime expiredAt)
    {
        Token = token;
        ExpiredAt = expiredAt;
    }

    public string Token { get; private set; }
    public DateTime ExpiredAt { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public bool IsDeleted { get; set; }
}