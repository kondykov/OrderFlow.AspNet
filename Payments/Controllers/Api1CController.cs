using Microsoft.AspNetCore.Mvc;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;

namespace Payments.Controllers;

[Route("api/1c")]
public class Api1CController : ApiController
{
    [HttpGet("payments")]
    public async Task<IActionResult> Get([FromQuery] PageQuery query)
    {
        return Ok();
    }
}