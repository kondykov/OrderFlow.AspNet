using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Interfaces;

public interface IOrderService
{
    Task<PaginationResponse<List<Order>>> GetOrdersAsync(int? pageNumber = 1, int? pageSize = 20);
    Task<Order> GetByIdAsync(int id);
    Task<Order> CreateAsync();
    Task<Order> UpdateAsync(UpdateOrderRequest request);
    Task<Order> NextStatusAsync(UpdateOrderRequest request);
    Task<List<OrderItem>> GetOrderItemsAsync(int orderId);
    Task<OrderItem> AddOrUpdateOrderItemAsync(AddOrUpdateOrderItemRequest request);
    Task<Dictionary<OrderStatus, string>> GetOrderStatusesAsync();
}