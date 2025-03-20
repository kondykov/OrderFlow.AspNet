namespace OrderFlow.Identity.Models.Request;

public class AddPermissionRequest
{
    public required string ClaimValue { get; set; }
    public required string RoleName { get; set; }
}