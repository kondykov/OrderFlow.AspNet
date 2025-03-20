using System.Security.Claims;

namespace OrderFlow.Shared.Models.Identity.DTOs;

public class RoleDto
{
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public List<ClaimDto> Claims { get; set; } = [];
}