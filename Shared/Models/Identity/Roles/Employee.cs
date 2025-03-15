namespace OrderFlow.Shared.Models.Identity.Roles;

public sealed class Employee : Role
{
    public Employee()
    {
        Name = "Employee";
        NormalizedName = Name.ToUpper();
        ParentRole = new Manager().ToString();
    }
}