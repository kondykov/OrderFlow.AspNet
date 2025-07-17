using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity.Devices;
using OrderFlow.Shared.Models.Identity.Roles;

namespace OrderFlow.Identity.Controllers;

[Authorize(Roles = nameof(Admin))]
[Route("identity/devices")]
public class DeviceController(IDeviceService service) : ApiController
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<Device?>))]
    public async Task<IActionResult> Register([FromBody] RegisterDeviceRequest request)
    {
        return Ok(new OperationResult<Device>
            {
                Data = await service.Add(request)
            }
        );
    }
    
    [HttpDelete("unregister")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    public async Task<IActionResult> UnRegister(int deviceId)
    {
        await service.Remove(deviceId);
        return Ok(new OperationResult());
    }


    [HttpGet("{deviceId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<Device>))]
    public async Task<IActionResult> GetById(int deviceId)
    {
        return Ok(new OperationResult<Device?>
        {
            Data = await service.GetById(deviceId)
        });
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<PaginationResponse<Device>>))]
    public async Task<IActionResult> Get([FromQuery] PageQuery query)
    {
        return Ok(new OperationResult<PaginationResponse<List<Device>>>
        {
            Data = await service.Get(query)
        });
    }
}