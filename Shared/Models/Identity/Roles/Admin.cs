namespace OrderFlow.Shared.Models.Identity.Roles;

public sealed class Admin : Role
{
    public Admin()
    {
        Name = "Admin";
        NormalizedName = Name.ToUpper();
    }
}