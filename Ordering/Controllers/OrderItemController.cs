using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Ordering.DTOs;

namespace OrderFlow.Ordering.Controllers;

[Authorize]
[Route("ordering/order-item")]
public class OrderItemController(IOrderService orderService) : ApiController
{
    [HttpPost("add-or-update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<OrderItemDto>))]
    public async Task<IActionResult> AddOrUpdate([FromBody] AddOrUpdateOrderItemRequest request)
    {
        return Ok(new OperationResult<OrderItem>
        {
            Data = await orderService.AddOrUpdateOrderItemAsync(request)
        });
    }

    [HttpGet("{orderId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<List<OrderItemDto>>))]
    public async Task<IActionResult> GetByOrderId(int orderId)
    {
        return Ok(new OperationResult<List<OrderItem>>
        {
            Data = await orderService.GetOrderItemsAsync(orderId)
        });
    }
}