using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Controllers;

[Authorize]
[Route("ordering/product/component")]
public class ProductComponentController(IProductService service) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ProductComponentRequest request)
    {
        return Ok(new OperationResult<Product>()
        {
            Data = await service.AddComponent(request.ProductId, request.ComponentId, request.Quantity)
        });
    }

    [HttpDelete]
    public async Task<IActionResult> Remove([FromBody] ProductComponentRequest request)
    {
        return Ok(new OperationResult<Product>()
        {
            Data = await service.RemoveComponent(request.ProductId, request.ComponentId, request.Quantity)
        });
    }
}