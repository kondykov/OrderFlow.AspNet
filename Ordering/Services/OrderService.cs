using OrderFlow.Identity.Interfaces;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Services;

public class OrderService(
    IUserService userService,
    IProductService productService,
    IOrdersRepository ordersRepository,
    IOrderItemsRepository orderItemsRepository
) : IOrderService
{
    public async Task<PaginationResponse<List<Order>>> GetOrders(int? pageNumber = 1, int? pageSize = 20)
    {
        return await ordersRepository.GetAllAsync(pageNumber, pageSize);
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

        if (!Enum.TryParse(request.OrderStatus, true, out OrderStatus orderStatus))
            throw new ArgumentException($"Статус {request.OrderStatus} не найден");
        order.UpdatedAt = DateTime.UtcNow;
        switch (order.Status)
        {
            case OrderStatus.Completed:
                throw new AccessDeniedException(
                    $"Нельзя изменить заказ со статусом \"{OrderStatus.Completed.GetDescription()}\"");
            case OrderStatus.Cancelled or OrderStatus.Rejected:
                throw new AccessDeniedException(
                    $"Нельзя изменить заказ со статусом \"{OrderStatus.Cancelled.GetDescription()}\" или \"{OrderStatus.Rejected.GetDescription()}\"");
        }

        switch (orderStatus)
        {
            case OrderStatus.New:
                throw new AccessDeniedException(
                    $"Невозможно изменить статус на \"{OrderStatus.New.GetDescription()}\"");
            case OrderStatus.Cancelled:
            case OrderStatus.Rejected:
                if (string.IsNullOrEmpty(request.Reason))
                    throw new ArgumentException(
                        $"Необходимо передать причину ({nameof(request.Reason)}) отклонения или отмены заказа");

                order.Status = orderStatus;
                order.Reason = request.Reason;
                return await ordersRepository.UpdateAsync(order);
            default:
                order.Status = orderStatus;
                return await ordersRepository.UpdateAsync(order);
        }
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
        if (order.Status == OrderStatus.New) order.Status = OrderStatus.Processing;
        if (order.Status != OrderStatus.Processing)
            throw new AccessDeniedException(
                $"Редактировать содержимое заказа допускается только у заказа со статусом \"{OrderStatus.Processing.GetDescription()}\"");
        var product = await productService.GetByIdAsync(request.ProductId);
        if (request.Quantity < 0) throw new ArgumentException("Количество не может быть отрицательным");
        var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == product.Id);
        if (orderItem == null)
        {
            if (!product.IsActive) throw new AccessDeniedException("Продукт недоступен для продажи");
            orderItem = new OrderItem
            {
                OrderId = order.Id,
                Quantity = request.Quantity,
                ProductId = product.Id
            };
            return await orderItemsRepository.AddAsync(orderItem);
        }

        orderItem.Quantity = request.Quantity;
        orderItem.UpdatedAt = DateTime.UtcNow;
        return await orderItemsRepository.UpdateAsync(orderItem);
    }

    public async Task<Dictionary<string, string>> GetOrderStatuses()
    {
        return Enum.GetValues(typeof(OrderStatus))
            .Cast<OrderStatus>()
            .ToDictionary(
                e => e.ToString(),
                e => e.GetDescription()
            );
    }
}