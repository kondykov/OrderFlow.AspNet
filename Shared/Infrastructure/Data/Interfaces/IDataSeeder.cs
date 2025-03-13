using Microsoft.Extensions.DependencyInjection;

namespace OrderFlow.Shared.Infrastructure.Data.Interfaces;

public interface IDataSeeder
{
    public Task SeedAsync(IServiceCollection serviceCollection);
}