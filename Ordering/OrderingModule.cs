using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Ordering.Infrastructure.Repositories;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Services;

namespace OrderFlow.Ordering;

public static class OrderingModule
{
    public static void AddOrderingModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
        builder.Services.AddScoped<IOrderItemsRepository, OrderItemsRepository>();
    }
}