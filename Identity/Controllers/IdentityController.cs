using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.DTOs;
using LoginRequest = OrderFlow.Identity.Models.Request.LoginRequest;
using RegisterRequest = OrderFlow.Identity.Models.Request.RegisterRequest;

namespace OrderFlow.Identity.Controllers;

[Authorize]
[Route("identity")]
public class IdentityController(IUserService service, IMapper mapper, RoleManager<Role> roleManager) : ApiController
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

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<AuthenticationResponse>))]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(new OperationResult());
        return Ok(new OperationResult<AuthenticationResponse>
        {
            Data = await service.RefreshTokenAsync(model)
        });
    }

    [HttpGet("get-user-info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    public async Task<IActionResult> GetInfo()
    {
        return Ok(new OperationResult<UserDto>
        {
            Data = await service.GetCurrentUserInfoAsync()
        });
    }

    [HttpPost("add-role")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> AddRole([FromBody] ChangeRoleRequest request)
    {
        return Ok(new OperationResult<UserDto>
        {
            Data = await service.AddRoleAsync(request)
        });
    }

    [HttpDelete("remove-role")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> RemoveRole([FromBody] ChangeRoleRequest request)
    {
        return Ok(new OperationResult<UserDto>
        {
            Data = await service.RemoveRoleAsync(request)
        });
    }

    [HttpPatch("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        return Ok(new OperationResult<UserDto>
        {
            Data = await service.ChangePasswordAsync(request)
        });
    }

    [HttpPost("security/add-permission")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> AddClaim([FromBody] AddPermissionRequest request)
    {
        var claimType = request.ClaimValue;
        var roleName = request.RoleName;

        if (!SystemClaims.TryGet(claimType, out var claim))
            return BadRequest(new OperationResult { Error = "Утверждение не найдено" });

        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
            return UnprocessableEntity(new OperationResult
            {
                Error = "Роль не найдена"
            });

        await service.AddClaimToRole(claim, role);
        return Ok(new OperationResult<UserDto>());
    }

    [HttpPost("security/remove-permission")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> RemoveClaim([FromBody] AddPermissionRequest request)
    {
        var claimValue = request.ClaimValue;
        var roleName = request.RoleName;

        if (!SystemClaims.TryGet(claimValue, out var claim))
            return BadRequest(new OperationResult { Error = "Утверждение не найдено" });

        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
            return UnprocessableEntity(new OperationResult
            {
                Error = "Роль не найдена"
            });

        await service.RemoveClaimFromRole(claim, role);
        return Ok(new OperationResult<UserDto>());
    }

    [HttpGet("security/get-roles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> GetRoles()
    {
        return Ok(new OperationResult<List<RoleDto>>
        {
            Data = await service.GetRolesAsync()
        });
    }
}
