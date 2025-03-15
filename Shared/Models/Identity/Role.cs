using Microsoft.AspNetCore.Identity;

namespace OrderFlow.Shared.Models.Identity;

public class Role : IdentityRole
{
    public virtual string? ParentRole { get; set; }
}