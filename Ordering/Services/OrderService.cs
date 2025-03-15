using OrderFlow.Identity.Interfaces;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Services;

public class OrderService(
    IUserService userService,
    IOrdersRepository ordersRepository,
    IOrderItemsRepository orderItemsRepository
) : IOrderService
{
    public async Task<List<Order>> GetOrders()
    {
        throw new NotImplementedException();
    }

    public async Task<Order> GetOrder(int id)
    {
        var order = await ordersRepository.GetByIdAsync(id);
        return order;
    }

    public async Task<Order> CreateOrder()
    {
        var user = await userService.GetCurrentUserAsync();

        var order = new Order
        {
            UserId = user.Id
        };
        return await ordersRepository.CreateAsync(order);
    }

    public async Task<Order> UpdateOrder(UpdateOrderRequest request)
    {
        var order = await ordersRepository.GetByIdAsync(request.Id);
        return await ordersRepository.UpdateAsync(order);
    }

    public Task<OrderItem> GetOrderItem(int id)
    {
        throw new NotImplementedException();
        var order = ordersRepository.GetByIdAsync(id);
    }

    public async Task<List<OrderItem>> GetOrderItems(int orderId)
    {
        var order = await ordersRepository.GetByIdAsync(orderId);
        return order.OrderItems;
    }

    public async Task<OrderItem> AddOrUpdateOrderItem(AddOrUpdateOrderItemRequest request)
    {
        var order = await ordersRepository.GetByIdAsync(request.OrderId);
        if (request.Quantity < 0) throw new ArgumentException("Количество не может быть отрицательным");
        var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == request.ProductId);
        if (orderItem == null)
        {
            orderItem = new OrderItem
            {
                OrderId = order.Id,
                Quantity = request.Quantity,
                ProductId = request.ProductId,
            };
            return await orderItemsRepository.AddAsync(orderItem);
        }
        orderItem.Quantity = request.Quantity;
        return await orderItemsRepository.UpdateAsync(orderItem);
    }
}