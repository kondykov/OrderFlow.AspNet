using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.DTOs;
using LoginRequest = OrderFlow.Identity.Models.Request.LoginRequest;
using RegisterRequest = OrderFlow.Identity.Models.Request.RegisterRequest;

namespace OrderFlow.Identity.Controllers;

[Route("identity")]
public class AuthenticationController(
    IAuthenticationService authService,
    ICurrentUserService currentUserService,
    IMapper mapper
) : ApiController
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<AuthenticationResponse>))]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
        return Ok(new OperationResult<AuthenticationResponse>
        {
            Data = await authService.LoginAsync(model)
        });
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(new OperationResult());
        await authService.RegisterAsync(model);
        return Ok(new OperationResult<bool> { Data = true });
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<AuthenticationResponse>))]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(new OperationResult());
        return Ok(new OperationResult<AuthenticationResponse>
        {
            Data = await authService.RefreshTokenAsync(model)
        });
    }
    
    [Authorize]
    [HttpGet("get-user-info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    public async Task<IActionResult> GetInfo()
    {
        return Ok(new OperationResult<User>
        {
            Data = await currentUserService.GetCurrentUserInfoAsync()
        });
    }
}