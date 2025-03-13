namespace OrderFlow.Shared.Models.Roles;

public sealed class Admin : Role
{
    public Admin()
    {
        Name = "Admin";
        NormalizedName = Name.ToUpper();
    }
}