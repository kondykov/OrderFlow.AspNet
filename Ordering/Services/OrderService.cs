using AutoMapper;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Ordering.DTOs;

namespace OrderFlow.Ordering.Services;

public class OrderService(
    IUserService userService,
    IProductService productService,
    IOrdersRepository ordersRepository,
    ICurrentUserService currentUserService,
    IOrderItemsRepository orderItemsRepository,
    IMapper mapper
) : IOrderService
{
    public async Task<PaginationResponse<List<OrderDto>>> GetOrdersAsync(int? pageNumber = 1, int? pageSize = 20)
    {
        await userService.RequireClaimAsync(SystemClaims.CanGetOrder);
        
        var paginationResponse = await ordersRepository.GetAllAsync(pageNumber, pageSize);
        return new PaginationResponse<List<OrderDto>>
        {
            Page = paginationResponse.Page,
            Pages = paginationResponse.Pages,
            PageSize = paginationResponse.PageSize,
            Data = mapper.Map<List<OrderDto>>(paginationResponse.Data),
        };
    }

    public async Task<Order> GetAsync(int id)
    {
        await userService.RequireClaimAsync(SystemClaims.CanGetOrder);
        
        return await ordersRepository.GetByIdAsync(id);
    }

    public async Task<OrderDto> CreateAsync()
    {
        await userService.RequireClaimAsync(SystemClaims.CanEditOrder);
        
        var user = await currentUserService.GetCurrentUserAsync();
        var order = new Order
        {
            UserId = user.Id
        };
        return mapper.Map<OrderDto>(await ordersRepository.CreateAsync(order));
    }

    public async Task<OrderDto> UpdateAsync(UpdateOrderRequest request)
    {
        await userService.RequireClaimAsync(SystemClaims.CanEditOrder);
        
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
                break;
            default:
                order.Status = orderStatus;
                break;
        }
        return mapper.Map<OrderDto>(await ordersRepository.UpdateAsync(order));
    }

    public Task<OrderItem> GetOrderItemAsync(int id)
    {
        throw new NotImplementedException();
        var order = ordersRepository.GetByIdAsync(id);
    }

    public async Task<List<OrderItemDto>> GetOrderItemsAsync(int orderId)
    {
        await userService.RequireClaimAsync(SystemClaims.CanGetOrder);
        
        var order = await ordersRepository.GetByIdAsync(orderId);
        return mapper.Map<List<OrderItemDto>>(order.OrderItems);
    }

    public async Task<OrderItemDto> AddOrUpdateOrderItemAsync(AddOrUpdateOrderItemRequest request)
    {
        await userService.RequireClaimAsync(SystemClaims.CanEditOrder);
        
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
            return mapper.Map<OrderItemDto>(await orderItemsRepository.AddAsync(orderItem));
        }

        orderItem.Quantity = request.Quantity;
        orderItem.UpdatedAt = DateTime.UtcNow;
        return mapper.Map<OrderItemDto>(await orderItemsRepository.UpdateAsync(orderItem));
    }

    public async Task<Dictionary<OrderStatus, string>> GetOrderStatusesAsync()
    {
        return Enum.GetValues(typeof(OrderStatus))
            .Cast<OrderStatus>()
            .ToDictionary(
                e => e,
                e => e.GetDescription()
            );
    }
}