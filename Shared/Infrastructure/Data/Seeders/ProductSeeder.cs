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
            new() { Id = 1, Name = "Продукт 1", Price = 127 },
            new() { Id = 2, Name = "Продукт 2", Price = 137 },
            new() { Id = 3, Name = "Продукт 3", Price = 149 },
            new() { Id = 4, Name = "Продукт с копейками", Price = (decimal)199.99 }
        ];

        foreach (var product in products.Where(product => !context.Products.Any(p => p.Id == product.Id)))
            context.Products.Add(product);
        await context.SaveChangesAsync();
    }
}