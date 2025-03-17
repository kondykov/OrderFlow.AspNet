using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Extensions;

namespace OrderFlow.Ordering.Controllers;

[Authorize]
[Route("ordering/order")]
public class OrderController(IOrderService service) : ApiController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create()
    {
        return Ok(await service.CreateOrder());
    }
    
    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] int orderId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(await service.GetOrder(orderId));
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateOrderRequest request)
    { 
        return Ok(await service.UpdateOrder(request));
    }

    [HttpGet("get-order-statuses")]
    public async Task<IActionResult> GetOrderStatuses()
    {
        return Ok(await service.GetOrderStatuses());
    }
}