using Microsoft.AspNetCore.Mvc;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Warehouse;
using OrderFlow.Stock.Interfaces;
using OrderFlow.Stock.Models;

namespace OrderFlow.Stock.Controllers;

[Route("warehouse/product")]
public class WarehouseController(IWarehouseService service) : ApiController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(new OperationResult<ProductReserve>()
        {
            Data = await service.GetProductReserveAsync(id)
        });
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] ProductReserveRequest request)
    {
        return Ok(new OperationResult<ProductReserve>()
        {
            Data = await service.AddProductReserveAsync(request.ProductId, request.Quantity)
        });
    }

    [HttpPost("take")]
    public async Task<IActionResult> Take([FromBody] ProductReserveRequest request)
    {
        return Ok(new OperationResult<ProductReserve>()
        {
            Data = await service.TakeProductReserveAsync(request.ProductId, request.Quantity)
        });
    }
}