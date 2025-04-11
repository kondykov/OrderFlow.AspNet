using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderFlow.Shared.Config;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Infrastructure.Data.Interfaces;
using OrderFlow.Shared.Infrastructure.Data.Seeders;
using OrderFlow.Shared.Mappers;

namespace OrderFlow.Shared;

public static class SharedModule
{
    public static void AddSharedModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DataContext>();
        builder.Services.Configure<ShaderConfig>(builder.Configuration.GetSection("Shared"));
        builder.Services.AddAutoMapper(typeof(MapperProfile));
        
        using var serviceProvider = builder.Services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<ShaderConfig>>().Value;
        if (options.UseSeeders)
        {
            var seeders = new List<IDataSeeder>
            {
                new RolesAndUsersSeeder(),
                new ProductSeeder(),
            };

            foreach (var seeder in seeders) seeder.SeedAsync(builder.Services).ConfigureAwait(true);
        }
    }
}