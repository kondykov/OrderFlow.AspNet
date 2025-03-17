using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Interfaces;

public interface IOrderService
{
    Task<List<Order>> GetOrders();
    Task<Order> GetOrder(int id);
    Task<Order> CreateOrder();
    Task<Order> UpdateOrder(UpdateOrderRequest request);
    
    Task<OrderItem> GetOrderItem(int id);
    Task<List<OrderItem>> GetOrderItems(int orderId);
    Task<OrderItem> AddOrUpdateOrderItem(AddOrUpdateOrderItemRequest request);
    Task<Dictionary<string, string>> GetOrderStatuses();
}