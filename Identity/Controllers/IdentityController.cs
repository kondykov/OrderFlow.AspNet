using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Identity.Abstractions;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Shared.Extensions;

namespace OrderFlow.Identity.Controllers;

[Route("identity")]
public class IdentityController(IUserService service) : ApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
        return Ok(await service.LoginAsync(model));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
        return Ok(await service.RegisterAsync(model));
    }
    
    [Authorize]
    [HttpGet("verify")]
    public async Task<IActionResult> Verify()
    {
        return Ok(true);
    }
}