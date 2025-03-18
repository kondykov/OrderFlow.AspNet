using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;

namespace OrderFlow.Ordering.Controllers;

[Authorize]
[Route("ordering/order-item")]
public class OrderItemController(IOrderService orderService) : ApiController
{
    [HttpPost("add-or-update")]
    public async Task<IActionResult> AddOrUpdate([FromBody] AddOrUpdateOrderItemRequest request)
    {
        return Ok(await orderService.AddOrUpdateOrderItem(request));
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetByOrderId(int orderId)
    {
        if (orderId <= 0)
            return BadRequest(new OperationResult
            {
                Error = "Необходимо передать идентификатор заказа"
            });

        return Ok(await orderService.GetOrderItems(orderId));
    }
}