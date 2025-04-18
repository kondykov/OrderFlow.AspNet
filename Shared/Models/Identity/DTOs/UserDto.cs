namespace OrderFlow.Shared.Models.Identity.DTOs;

public class UserDto
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public string? UserName { get; set; }
    public string? NormalizedUserName { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public bool IsTwoFactorEnabled { get; set; } = false;
    public IList<RoleDto> Roles { get; set; }
}