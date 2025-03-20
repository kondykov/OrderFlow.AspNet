using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity.DTOs;

namespace OrderFlow.Identity.Controllers;

[Authorize]
[Route("identity")]
public class IdentityController(IUserService service, IMapper mapper) : ApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<AuthenticationResponse>))]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
        return Ok(new OperationResult<AuthenticationResponse>
        {
            Data = await service.LoginAsync(model)
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(new OperationResult());
        await service.RegisterAsync(model);
        return Ok(new OperationResult());
    }

    [HttpGet("get-user-info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    public async Task<IActionResult> GetInfo()
    {
        var user = await service.GetCurrentUserAsync();
        return Ok(new OperationResult<UserDto>
        {
            Data = mapper.Map<UserDto>(user)
        });
    }

    /*[HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody] ChangeRoleRequest request)
    {
        return Ok(await service.AddRoleAsync(request));
    }

    [HttpDelete("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] ChangeRoleRequest request)
    {
        return Ok(await service.RemoveRoleAsync(request));
    }*/

    [HttpPatch("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        return Ok(new OperationResult<UserDto>
        {
            Data = await service.ChangePasswordAsync(request)
        });
    }
}