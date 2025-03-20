using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity.Roles;
using OrderFlow.Shared.Models.Ordering.DTOs;

namespace OrderFlow.Ordering.Controllers;

[Authorize(Roles = nameof(Admin))]
[Route("ordering/product")]
public class ProductController(IProductService service) : ApiController
{
    [HttpGet("get-all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<PaginationResponse<List<ProductDto>>>))]
    public async Task<IActionResult> GetAll(int? page = 1, int? pageSize = 20)
    {
        return Ok(await service.GetAllAsync(page, pageSize));
    }

    [HttpGet("get-active")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<PaginationResponse<List<ProductDto>>>))]
    public async Task<IActionResult> GetAllActive()
    {
        return Ok(await service.GetAllActiveAsync());
    }

    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<ProductDto>))]
    public async Task<IActionResult> Add([FromBody] AddProductRequest request)
    {
        return Ok(await service.AddAsync(request));
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<ProductDto>))]
    public async Task<IActionResult> Update([FromBody] UpdateProductRequest request)
    {
        return Ok(await service.UpdateAsync(request));
    }

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<ProductDto>))]
    public async Task<IActionResult> Delete([FromBody] RemoveProductRequest request)
    {
        await service.DeleteAsync(request);
        return NoContent();
    }
}