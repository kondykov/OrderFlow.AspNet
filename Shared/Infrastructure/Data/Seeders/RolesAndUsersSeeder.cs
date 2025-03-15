using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Shared.Infrastructure.Data.Interfaces;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.Roles;

namespace OrderFlow.Shared.Infrastructure.Data.Seeders;

public class RolesAndUsersSeeder : IDataSeeder
{
    public async Task SeedAsync(IServiceCollection serviceCollection)
    {
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var roles = new List<Role>
        {
            new Admin(),
            new Manager(),
            new Employee(),
            new Terminal(),
            new Client(),
        };
        
        foreach (var role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role.Name);
            if (!roleExist) await roleManager.CreateAsync(role);
        }

        var adminUser = new User
        {
            UserName = "Administrator", Email = "admin@example.com"
        };

        var userExist = await userManager.FindByEmailAsync(adminUser.Email);
        if (userExist == null)
        {
            var createAdminResult = await userManager.CreateAsync(adminUser, "Pa$$w0rd");
            if (createAdminResult.Succeeded) await userManager.AddToRoleAsync(adminUser, new Admin().ToString());
        }
    }
}