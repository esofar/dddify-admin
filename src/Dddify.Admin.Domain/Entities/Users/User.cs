using Dddify.Admin.Domain.Events.Users;
using Dddify.Identity;
using System.Security.Claims;

namespace Dddify.Admin.Domain.Entities.Users;

public class User : FullAuditedAggregateRoot<Guid>, IHasConcurrencyStamp, ISoftDeletable
{
    public User(string fullName, string? nickName, 
        UserGender gender, DateOnly? birthDate, 
        string email, string phoneNumber, Guid organizationId, 
        string type, string passwordHash)
    {
        FullName = fullName;
        NickName = nickName;
        Gender = gender;
        BirthDate = birthDate;
        Email = email;
        PhoneNumber = phoneNumber;
        OrganizationId = organizationId;
        Type = type;
        PasswordHash = passwordHash;

        AddDomainEvent(new UserCreatedDomainEvent(Id, FullName, Email, PhoneNumber));
    }

    private User() { }

    public string FullName { get; private set; }
    public string? NickName { get; private set; }
    public string? Avatar { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public UserGender Gender { get; private set; }
    public string Email { get; private set; }
    public bool EmailVerified { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool PhoneNumberVerified { get; private set; }
    public string PasswordHash { get; private set; }
    public string Type { get; private set; }
    public UserStatus Status { get; private set; }
    public string? ConcurrencyStamp { get; set; }
    public bool IsDeleted { get; set; }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; }
    public List<UserRefreshToken> RefreshTokens { get; private set; }
    public List<Role> Roles { get; private set; }
    public List<UserRole> UserRoles { get; private set; }

    public void Update(string fullName, string? nickName, UserGender gender,
        DateOnly? birthDate, string email, string phoneNumber,
        Guid organizationId, string type)
    {
        FullName = fullName;
        NickName = nickName;
        Gender = gender;
        BirthDate = birthDate;
        Email = email;
        PhoneNumber = phoneNumber;
        OrganizationId = organizationId;
        Type = type;
    }

    public void AddRefreshToken(string token, DateTime expiredAt)
        => RefreshTokens.Add(new(token, expiredAt));

    public void RevokeRefreshToken(string refreshToken)
        => RefreshTokens.RemoveAll(c => c.Token == refreshToken);

    public IEnumerable<Claim> JwtClaims()
    {
        yield return new Claim(DefaultClaimTypes.UserId, Id.ToString());
        yield return new Claim(DefaultClaimTypes.Email, Email);
        yield return new Claim(DefaultClaimTypes.EmailVerified, EmailVerified.ToString(), ClaimValueTypes.Boolean);
        yield return new Claim(DefaultClaimTypes.PhoneNumber, PhoneNumber);
        yield return new Claim(DefaultClaimTypes.PhoneNumberVerified, PhoneNumberVerified.ToString(), ClaimValueTypes.Boolean);
    }
}