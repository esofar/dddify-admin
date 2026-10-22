namespace Dddify.Admin.Infrastructure.Email;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public bool UseDevelopmentSender { get; set; }

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 587;

    public bool EnableSsl { get; set; } = true;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string From { get; set; } = string.Empty;

    public string DisplayName { get; set; } = "Dddify Admin";
}
