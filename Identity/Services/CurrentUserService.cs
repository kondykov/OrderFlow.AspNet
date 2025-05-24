using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Shared.Models.Identity;

namespace OrderFlow.Identity.Services;

public class CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager,
    RoleManager<Role> roleManager
    ) : ICurrentUserService
{
    public async Task<User> GetCurrentUserAsync()
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;
        if (claimsPrincipal == null || !claimsPrincipal.Identity!.IsAuthenticated)
            throw new UnauthorizedAccessException();
        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userManager.FindByIdAsync(userId ?? throw new UnauthorizedAccessException());
        if (user == null) throw new UnauthorizedAccessException();
        
        return user;
    }

    public async Task<User> GetCurrentUserInfoAsync()
    {
        var user = await GetCurrentUserAsync();
        var rolesString = await userManager.GetRolesAsync(user);
        var roles = new List<IList<Claim>>();
        foreach (var roleString in rolesString)
        {
            var roleEntity = await roleManager.FindByNameAsync(roleString);
            if (roleEntity == null) continue;
            roles.Add(await roleManager.GetClaimsAsync(roleEntity));
        }

        return user;
    }
}