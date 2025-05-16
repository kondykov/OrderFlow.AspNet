using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.DTOs;

namespace OrderFlow.Identity.Controllers;

[Authorize]
[Route("identity")]
public class SecurityController(
    IUserService userService,
    RoleManager<Role> roleManager,
    IMapper mapper
    ) : ApiController
{
    [HttpPost("add-role")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> AddRole([FromBody] ChangeRoleRequest request)
    {
        return Ok(new OperationResult<UserDto>
        {
            Data = await userService.AddRoleAsync(request)
        });
    }

    [HttpDelete("remove-role")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> RemoveRole([FromBody] ChangeRoleRequest request)
    {
        return Ok(new OperationResult<UserDto>
        {
            Data = await userService.RemoveRoleAsync(request)
        });
    }

    [HttpPatch("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        return Ok(new OperationResult<UserDto>
        {
            Data = await userService.ChangePasswordAsync(request)
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

        await userService.AddClaimToRole(claim, role);
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

        await userService.RemoveClaimFromRole(claim, role);
        return Ok(new OperationResult<UserDto>());
    }

    [HttpGet("security/get-roles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<UserDto>))]
    public async Task<IActionResult> GetRoles()
    {
        return Ok(new OperationResult<List<RoleDto>>
        {
            Data = await userService.GetRolesAsync()
        });
    }
}
