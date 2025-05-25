using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Stock.Interfaces;
using OrderFlow.Stock.Repositories;
using OrderFlow.Stock.Services;

namespace OrderFlow.Stock;

public static class WarehouseModule
{
    public static void AddWarehouseModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IProductReserveSettingsRepository, ProductReserveSettingsRepository>();
        builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        builder.Services.AddScoped<IWarehouseService, WarehouseService>();
    }
}