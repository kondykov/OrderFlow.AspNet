using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Shared.Extensions;

namespace OrderFlow.Identity.Controllers;

[Authorize]
[Route("identity")]
public class IdentityController(IUserService service) : ApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
        return Ok(await service.LoginAsync(model));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
        return Ok(await service.RegisterAsync(model));
    }

    [HttpGet("verify")]
    public async Task<IActionResult> Verify()
    {
        return Ok(true);
    }

    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody] ChangeRoleRequest request)
    {
        return Ok(await service.AddRoleAsync(request));
    }

    [HttpDelete("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] ChangeRoleRequest request)
    {
        return Ok(await service.RemoveRoleAsync(request));
    }
}