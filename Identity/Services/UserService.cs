using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderFlow.Identity.Config;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models.Identity;

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
            throw new EntityNotFoundException("Пользователь не найден");

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            throw new EntityNotFoundException("Неверный пароль");

        return new AuthenticationResponse
        {
            AccessToken = GenerateJwtToken(user),
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(identityConfig.Value.Jwt.ExpiryInMinutes)
        };
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var userExists = await userManager.FindByEmailAsync(request.Email);
        if (userExists != null)
            throw new EntityNotFoundException("Такой пользователь уже существует");

        var user = await userManager.CreateAsync(new User
        {
            Email = request.Email,
            UserName = request.UserName
        }, request.Password);

        if (user.Succeeded) return true;

        throw new Exception($"Произошла ошибка при создании пользователя: {string.Join(", ", user.Errors)}");
    }


    public async Task<User> GetCurrentUserAsync()
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;
        if (claimsPrincipal == null || !claimsPrincipal.Identity.IsAuthenticated)
            throw new UnauthorizedAccessException();
        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) throw new UnauthorizedAccessException();
        return user;
    }

    public async Task<UserDto> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await GetCurrentUserAsync();

        var passwordVerification =
            userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword);
        if (passwordVerification != PasswordVerificationResult.Success)
            throw new AccessDeniedException("Переданный пароль не соответсвутет текущему");

        await userManager.RemovePasswordAsync(user);
        await userManager.AddPasswordAsync(user, request.Password);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            NormalizedEmail = user.NormalizedEmail,
            UserName = user.UserName,
            NormalizedUserName = user.NormalizedUserName,
            IsEmailConfirmed = user.EmailConfirmed,
            IsTwoFactorEnabled = user.TwoFactorEnabled,
            Roles = await userManager.GetRolesAsync(user)
        };
    }

    public async Task<UserDto> AddRoleAsync(ChangeRoleRequest request)
    {
        var currentUser = await GetCurrentUserAsync();
        var user = userManager.Users.FirstOrDefault(u => u.Id == request.Id);
        if (user == null) throw new EntityNotFoundException("Пользователь не найден");
        var role = await roleManager.FindByNameAsync(request.Role);
        if (role == null) throw new EntityNotFoundException("Роль не найдена");
        if (user == currentUser) throw new AccessDeniedException("Вы не можете изменить себе роль");
        var result = await userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded) throw new DataException($"Ошибка: {string.Join(", ", result.Errors)}");
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            NormalizedEmail = user.NormalizedEmail,
            UserName = user.UserName,
            NormalizedUserName = user.NormalizedUserName,
            IsEmailConfirmed = user.EmailConfirmed,
            IsTwoFactorEnabled = user.TwoFactorEnabled,
            Roles = await userManager.GetRolesAsync(user)
        };
    }

    public async Task<UserDto> RemoveRoleAsync(ChangeRoleRequest request)
    {
        var currentUser = await GetCurrentUserAsync();
        var user = userManager.Users.FirstOrDefault(u => u.Id == request.Id);
        if (user == null) throw new EntityNotFoundException("Пользователь не найден");
        var role = await roleManager.FindByNameAsync(request.Role);
        if (role == null) throw new EntityNotFoundException("Роль не найдена");
        if (user == currentUser) throw new AccessDeniedException("Вы не можете изменить себе роль");
        var result = await userManager.RemoveFromRoleAsync(user, request.Role);
        if (!result.Succeeded) throw new DataException($"Ошибка: {string.Join(", ", result.Errors)}");
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            NormalizedEmail = user.NormalizedEmail,
            UserName = user.UserName,
            NormalizedUserName = user.NormalizedUserName,
            IsEmailConfirmed = user.EmailConfirmed,
            IsTwoFactorEnabled = user.TwoFactorEnabled,
            Roles = await userManager.GetRolesAsync(user)
        };
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
        if (identityConfig.Value.Jwt.Secret == null) throw new Exception("Secret Key is missing");

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