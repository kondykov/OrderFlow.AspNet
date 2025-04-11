using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Ordering.DTOs;

namespace OrderFlow.Ordering.Interfaces;

public interface IOrderService
{
    Task<PaginationResponse<List<OrderDto>>> GetOrdersAsync(int? pageNumber = 1, int? pageSize = 20);
    Task<Order> GetAsync(int id);
    Task<OrderDto> CreateAsync();
    Task<OrderDto> UpdateAsync(UpdateOrderRequest request);
    
    Task<OrderItem> GetOrderItemAsync(int id);
    Task<List<OrderItemDto>> GetOrderItemsAsync(int orderId);
    Task<OrderItemDto> AddOrUpdateOrderItemAsync(AddOrUpdateOrderItemRequest request);
    Task<Dictionary<OrderStatus, string>> GetOrderStatusesAsync();
}