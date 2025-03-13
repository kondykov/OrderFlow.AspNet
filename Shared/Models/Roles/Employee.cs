namespace OrderFlow.Shared.Models.Roles;

public sealed class Employee : Role
{
    public Employee()
    {
        Name = "Employee";
        NormalizedName = Name.ToUpper();
        ParentRole = new Manager().ToString();
    }
}