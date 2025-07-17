using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderFlow.Identity.Config;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.Devices;
using OrderFlow.Shared.Models.Identity.Roles;

namespace OrderFlow.Identity.Services;

public class TokenService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IRefreshTokensRepository refreshTokensRepository,
    IOptions<IdentityConfig> identityConfig
) : ITokenService
{
    public string GenerateAccessToken(User user)
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
    
    public string GenerateAccessToken(Device device)
    {
        if (identityConfig.Value.Jwt.Secret == null) throw new Exception("Secret Key is missing");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, nameof(Terminal))
        };

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

    public async Task<RefreshToken> GenerateRefreshToken(User user)
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