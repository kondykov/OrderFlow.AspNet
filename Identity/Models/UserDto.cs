namespace OrderFlow.Identity.Models;

public class UserDto
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public string? UserName { get; set; }
    public string? NormalizedUserName { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public bool IsTwoFactorEnabled { get; set; } = false;
    public IList<string> Roles { get; set; }
}