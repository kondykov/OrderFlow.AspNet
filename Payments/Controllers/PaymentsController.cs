using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Ordering;
using Payments.Interfaces;

namespace Payments.Controllers;

[Authorize]
[Route("payments")]
public class PaymentsController(IPaymentService service, IOrderService orderService, IMapper mapper) : ApiController
{
    [HttpGet("get/{id}")]
    public async Task<ActionResult> GetById(int? id)
    {
        if (!id.HasValue) return UnprocessableEntity(new OperationResult()
        {
            Error = "Необходимо указать идентификатор платежа"
        });
        
        return Ok();
    }
    [HttpGet("debug")]
    public async Task<IActionResult> Debug()
    {
        var order = await orderService.GetByIdAsync(2);
        return Ok(service.Create(mapper.Map<Order>(order), 123));
    }
}