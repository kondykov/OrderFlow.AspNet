using Microsoft.AspNetCore.Identity;

namespace OrderFlow.Shared.Models;

public class Role : IdentityRole
{
    public virtual string? ParentRole { get; set; }
}