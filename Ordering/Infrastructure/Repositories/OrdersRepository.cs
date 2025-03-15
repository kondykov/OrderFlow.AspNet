using Microsoft.EntityFrameworkCore;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Infrastructure.Repositories;

public class OrdersRepository(DataContext context) : IOrdersRepository
{
    public async Task<Order> CreateAsync(Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return order;
    }


    public async Task<Order> UpdateAsync(Order order)
    {
        var orderExists = context.Orders.FirstOrDefault(o => o.Id == order.Id);
        if (orderExists == null) throw new EntityNotFoundException($"Заказ с идентификатором {order.Id} не найден");
        context.Entry(orderExists).CurrentValues.SetValues(order);
        await context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> FindByIdAsync(int id)
    {
        return await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> GetByIdAsync(int id)
    {
        var order = await FindByIdAsync(id);
        if (order == null) throw new EntityNotFoundException($"Заказ с идентификатором {id} не найден");
        return order;
    }
}