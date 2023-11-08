namespace Dddify.Admin.Application.Dtos.Users;

public record UserDetailDto : UserDto
{
    public string? ConcurrencyStamp { get; set; }
}