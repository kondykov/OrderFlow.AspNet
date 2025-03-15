namespace OrderFlow.Shared.Models.Identity.Roles;

public sealed class Terminal : Role
{
    public Terminal()
    {
        Name = "Terminal";
        NormalizedName = Name.ToUpper();
        ParentRole = new Manager().ToString();
    }
}