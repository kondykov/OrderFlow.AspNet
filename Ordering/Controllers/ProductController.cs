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
[Route("ordering/product")]
public class ProductController(IProductService service) : ApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<PaginationResponse<List<ProductDto>>>))]
    public async Task<IActionResult> GetAll(int? page = 1, int? pageSize = 20, bool? isActive = null, bool? isSellable = null)
    {
        return Ok(new OperationResult<PaginationResponse<List<Product>>>()
        {
            Data = await service.GetAllAsync(page, pageSize, isActive, isSellable)
        });
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<PaginationResponse<List<ProductDto>>>))]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(new OperationResult<Product>()
        {
            Data = await service.GetByIdAsync(id)
        });
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<ProductDto>))]
    public async Task<IActionResult> Add([FromBody] AddProductRequest request)
    {
        return Ok(new OperationResult<Product>()
        {
            Data = await service.AddAsync(request)
        });
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<ProductDto>))]
    public async Task<IActionResult> Update([FromBody] UpdateProductRequest request)
    {
        return Ok(new OperationResult<Product>()
        {
            Data = await service.UpdateAsync(request)
        });
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<ProductDto>))]
    public async Task<IActionResult> Delete([FromBody] RemoveProductRequest request)
    {
        await service.DeleteAsync(request);
        return NoContent();
    }
}