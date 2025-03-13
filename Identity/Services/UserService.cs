using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderFlow.Identity.Abstractions;
using OrderFlow.Identity.Config;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models;

namespace OrderFlow.Identity.Services;

public class UserService(
    IHttpContextAccessor httpContextAccessor,
    IPasswordHasher<User> passwordHasher,
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IOptions<IdentityConfig> identityConfig
) : IUserService
{
    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new EntityNotFoundException($"User with email {request.Email} does not exist");

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            throw new EntityNotFoundException($"User with email {request.Email} does not exist");

        return new AuthenticationResponse
        {
            AccessToken = GenerateJwtToken(user)
        };
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var userExists = await userManager.FindByEmailAsync(request.Email);
        if (userExists != null)
            throw new EntityNotFoundException($"User with email {request.Email} does not exist");

        var user = await userManager.CreateAsync(new User
        {
            Email = request.Email,
            UserName = request.UserName
        }, request.Password);

        if (user.Succeeded) return true;

        throw new Exception($"User with email {request.Email} does not exist");
    }


    public async Task<User> GetCurrentUserAsync()
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;
        if (claimsPrincipal == null || !claimsPrincipal.Identity.IsAuthenticated)
            throw new UnauthorizedAccessException();
        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return await userManager.FindByIdAsync(userId);
    }

    public async Task<bool> HasRoleAsync(Role? role)
    {
        var user = await GetCurrentUserAsync();
        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(role!.ToString())) return true;
        role = await roleManager.FindByNameAsync(role.ToString());
        while (role != null)
        {
            if (roles.Contains(role.Name)) return true;
            role = await roleManager.FindByIdAsync(role.ParentRole!);
        }

        return false;
    }

    public async Task<bool> HasRoleAsync(List<Role> roles)
    {
        foreach (var role in roles)
            if (await HasRoleAsync(role))
                return true;
        return false;
    }

    private string GenerateJwtToken(User user)
    {
        if (identityConfig.Value.Jwt.Secret == null) throw new Exception("Secret Key is missing.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id)
        };
        claims.AddRange(userManager.GetRolesAsync(user).GetAwaiter().GetResult()
            .Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(identityConfig.Value.Jwt.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            identityConfig.Value.Jwt.Issuer,
            identityConfig.Value.Jwt.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}