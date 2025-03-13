using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Infrastructure.Data.Interfaces;
using OrderFlow.Shared.Infrastructure.Data.Seeders;
using OrderFlow.Shared.Models;

namespace OrderFlow.Shared;

public static class SharedModule
{
    public static void AddSharedModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DataContext>();
        
        var seeders = new List<IDataSeeder>()
        {
            new RolesAndUsersSeeder(),
        };

        foreach (var seeder in seeders)
        {
            seeder.SeedAsync(builder.Services).ConfigureAwait(true);
        }
    }
}