namespace Dddify.Admin.Application.Dtos.Users;

public record UserProfileDto
{
    public string Name { get; set; }

    public string? NickName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string? Avatar { get; set; }

    public UserGender? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string Access { get; set; } = "admin";
    public string Address { get; set; } = "西湖区工专路 77 号";
    public string Country { get; set; } = "China";
    public string Signature { get; set; } = "海纳百川，有容乃大";
    public string Title { get; set; } = "交互专家";
    public string Group { get; set; } = "蚂蚁金服－某某某事业群－某某平台部－某某技术部－UED";
}