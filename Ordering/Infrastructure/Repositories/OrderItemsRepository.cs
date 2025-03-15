using OrderFlow.Ordering.Interfaces;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Infrastructure.Repositories;

public class OrderItemsRepository(DataContext context) : IOrderItemsRepository
{
    public async Task<OrderItem?> FindByIdAsync(int id)
    {
        return context.OrderItems.FirstOrDefault(oi => oi.Id == id);
    }

    public async Task<OrderItem> AddAsync(OrderItem orderItem)
    {
        context.OrderItems.Add(orderItem);
        await context.SaveChangesAsync();
        return orderItem;
    }

    public async Task<OrderItem> UpdateAsync(OrderItem orderItem)
    {
        var orderItemExists = context.OrderItems.FirstOrDefault(o => o.OrderId == orderItem.OrderId);
        if (orderItemExists == null) throw new EntityNotFoundException($"Элемент заказа {orderItem.OrderId} с идентификатором {orderItem.Id} не найден");
        context.Entry(orderItemExists).CurrentValues.SetValues(orderItem);
        await context.SaveChangesAsync();
        return orderItem;
    }

    public async Task<List<OrderItem>> GetAllAsync(int orderId)
    {
        return context.OrderItems.Where(oi => oi.OrderId == orderId).ToList();
    }

    public async Task DeleteAsync(OrderItem orderItem)
    {
        throw new NotImplementedException();
    }
}