using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Shared.Infrastructure.Data.Interfaces;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Shared.Infrastructure.Data.Seeders;

public class ProductSeeder : IDataSeeder
{
    public async Task SeedAsync(IServiceCollection serviceCollection)
    {
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        List<Product> products =
        [
            new() { Name = "Продукт 1", Price = 127 },
            new() { Name = "Продукт 2", Price = 137 },
            new() { Name = "Продукт 3", Price = 149 },
            new() { Name = "Продукт с копейками", Price = (decimal)199.99 },
            new() { Name = "Неактивный продукт с копейками", Price = (decimal)199.99, IsActive = false }
        ];

        foreach (var product in products.Where(product => !context.Products.Any(p => p.Name == product.Name)))
            await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
    }
}