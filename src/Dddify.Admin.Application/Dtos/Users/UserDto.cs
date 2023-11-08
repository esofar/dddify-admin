namespace Dddify.Admin.Application.Dtos.Users;

public record UserDto : IRegister
{
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public string? NickName { get; set; }

    public UserGender Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string? Avatar { get; set; }

    public Guid OrganizationId { get; set; }

    public string OrganizationName { get; set; }

    public UserStatus Status { get; set; }

    public string Type { get; set; }

    public void Register(TypeAdapterConfig config)
    {
        config.ForType<User, UserDto>()
           .Map(dest => dest.OrganizationName, src => src.Organization.Name);
    }
}