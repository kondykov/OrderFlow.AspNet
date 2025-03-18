using Microsoft.EntityFrameworkCore;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models;
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

    public async Task<PaginationResponse<List<Order>>> GetAllAsync(int? pageNumber = 1, int? pageSize = 20)
    {
        var totalCount = await context.Orders.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize.Value);
        var orders = await context.Orders
            .Include(o => o.OrderItems)
            .Skip((int)((pageNumber - 1) * pageSize)!)
            .Take((int)pageSize!)
            .ToListAsync();

        return new PaginationResponse<List<Order>>
        {
            Data = orders,
            Page = pageNumber ?? 1,
            PageSize = pageSize ?? 20,
            Pages = totalPages,
        };
    }
}