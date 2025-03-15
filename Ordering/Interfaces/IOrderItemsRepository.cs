using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Interfaces;

public interface IOrderItemsRepository
{
    Task<OrderItem?> FindByIdAsync(int id);
    Task<OrderItem> AddAsync(OrderItem orderItem);
    Task<OrderItem> UpdateAsync(OrderItem orderItem);
    Task<List<OrderItem>> GetAllAsync(int orderId);
    Task DeleteAsync(OrderItem orderItem);
}