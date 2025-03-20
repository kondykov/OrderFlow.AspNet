using OrderFlow.Ordering.Models;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Ordering.DTOs;

namespace OrderFlow.Ordering.Interfaces;

public interface IOrderService
{
    Task<PaginationResponse<List<OrderDto>>> GetOrders(int? pageNumber = 1, int? pageSize = 20);
    Task<OrderDto> Get(int id);
    Task<OrderDto> Create();
    Task<OrderDto> Update(UpdateOrderRequest request);
    
    Task<OrderItem> GetOrderItem(int id);
    Task<List<OrderItemDto>> GetOrderItems(int orderId);
    Task<OrderItemDto> AddOrUpdateOrderItem(AddOrUpdateOrderItemRequest request);
    Task<Dictionary<OrderStatus, string>> GetOrderStatuses();
}