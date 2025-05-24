using AutoMapper;
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
[Route("ordering/order")]
public class OrderController(IOrderService service) : ApiController
{
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<OrderDto>))]
    public async Task<IActionResult> Create()
    {
        return Ok(await service.CreateAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<OrderDto>))]
    public async Task<IActionResult> Get(int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(new OperationResult<Order>
        {
            Data = await service.GetByIdAsync(id)
        });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<PaginationResponse<List<OrderDto>>>))]
    public async Task<IActionResult> GetAll([FromQuery] int? page = 1, [FromQuery] int? pageSize = 20)
    {
        return Ok(new OperationResult<PaginationResponse<List<Order>>>
        {
            Data = await service.GetOrdersAsync(page, pageSize)
        });
    }

    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<OrderDto>))]
    public async Task<IActionResult> Update([FromBody] UpdateOrderRequest request)
    {
        return Ok(new OperationResult<Order>
        {
            Data = await service.UpdateAsync(request)
        });
    }

    [HttpGet("get-order-statuses")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<Dictionary<string, string>>))]
    public async Task<IActionResult> GetOrderStatuses()
    {
        return Ok(new OperationResult<Dictionary<OrderStatus, string>>
        {
            Data = await service.GetOrderStatusesAsync()
        });
    }
}