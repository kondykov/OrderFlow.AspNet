using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderFlow.Identity.Config;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models.Identity;
using LoginRequest = OrderFlow.Identity.Models.Request.LoginRequest;
using RegisterRequest = OrderFlow.Identity.Models.Request.RegisterRequest;

namespace OrderFlow.Identity.Services;

public class AuthenticationService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IRefreshTokensRepository refreshTokensRepository,
    IOptions<IdentityConfig> identityConfig
) : IAuthenticationService
{
    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new EntityNotFoundException("Пользователь не найден");

        var passwordVerificationResult =
            userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            throw new ArgumentException("Неверный пароль");

        var newRefreshToken = await GenerateRefreshTokenAsync(user);
        return new AuthenticationResponse
        {
            AccessToken = GenerateJwtToken(user),
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(identityConfig.Value.Jwt.ExpiryInMinutes),
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiration = newRefreshToken.ExpiryDate
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

        throw new ArgumentException($"Произошла ошибка при создании пользователя: {string.Join(", ", user.Errors)}");
    }

    public async Task<AuthenticationResponse> RefreshTokenAsync(RefreshRequest request)
    {
        var refreshToken = await refreshTokensRepository.GetAsync(request.RefreshToken);
        await refreshTokensRepository.RevokeAsync(refreshToken.Token);
        var user = await userManager.FindByIdAsync(refreshToken.UserId);
        var newRefreshToken = await GenerateRefreshTokenAsync(user);
        
        return new AuthenticationResponse
        {
            AccessToken = GenerateJwtToken(user),
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(identityConfig.Value.Jwt.ExpiryInMinutes),
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiration = newRefreshToken.ExpiryDate
        };
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
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(User user)
    {
        var token = Guid.NewGuid().ToString();
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
        await refreshTokensRepository.AddAsync(refreshToken);
        return refreshToken;
    }
}