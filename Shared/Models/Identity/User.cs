using Microsoft.AspNetCore.Identity;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Shared.Models.Identity;

public class User : IdentityUser
{
    public List<Order>? Orders { get; set; }
}