using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderFlow.Identity.Config;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.DTOs;
using OrderFlow.Shared.Models.Identity.Roles;
using LoginRequest = OrderFlow.Identity.Models.Request.LoginRequest;
using RegisterRequest = OrderFlow.Identity.Models.Request.RegisterRequest;

namespace OrderFlow.Identity.Services;

public class UserService(
    IMapper mapper,
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IHttpContextAccessor httpContextAccessor,
    IRefreshTokensRepository refreshTokensRepository,
    IOptions<IdentityConfig> identityConfig
) : IUserService
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

        throw new Exception($"Произошла ошибка при создании пользователя: {string.Join(", ", user.Errors)}");
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

    public async Task<UserDto> GetCurrentUserInfoAsync()
    {
        var user = await GetCurrentUserAsync();
        var rolesString = await userManager.GetRolesAsync(user);
        var roles = new List<RoleDto>();
        foreach (var roleString in rolesString)
        {
            var roleEntity = await roleManager.FindByNameAsync(roleString);
            if (roleEntity == null) continue;
            var claims = await roleManager.GetClaimsAsync(roleEntity);
            var roleDto = mapper.Map<RoleDto>(roleEntity);
            roleDto.Claims = mapper.Map<List<ClaimDto>>(claims);
            roles.Add(roleDto);
        }

        var userDto = mapper.Map<UserDto>(user);
        var rolesDto = mapper.Map<IList<RoleDto>>(roles);
        userDto.Roles = rolesDto;
        return userDto;
    }

    public async Task<UserDto> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await GetCurrentUserAsync();

        var passwordVerification =
            userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword);
        if (passwordVerification != PasswordVerificationResult.Success) throw new ArgumentException("Неверный пароль");

        await userManager.RemovePasswordAsync(user);
        await userManager.AddPasswordAsync(user, request.Password);
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> AddRoleAsync(ChangeRoleRequest request)
    {
        await RequireClaimAsync(SystemClaims.CanChangeRole);

        var currentUser = await GetCurrentUserAsync();
        var user = userManager.Users.FirstOrDefault(u => u.Id == request.Id);
        if (user == null) throw new EntityNotFoundException("Пользователь не найден");
        var role = await roleManager.FindByNameAsync(request.Role);
        if (role == null) throw new EntityNotFoundException("Роль не найдена");
        if (user == currentUser) throw new AccessDeniedException("Вы не можете изменить себе роль");
        var result = await userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded) throw new DataException($"Ошибка: {string.Join(", ", result.Errors)}");
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> RemoveRoleAsync(ChangeRoleRequest request)
    {
        await RequireClaimAsync(SystemClaims.CanChangeRole);

        var currentUser = await GetCurrentUserAsync();
        var user = userManager.Users.FirstOrDefault(u => u.Id == request.Id);
        if (user == null) throw new EntityNotFoundException("Пользователь не найден");
        var role = await roleManager.FindByNameAsync(request.Role);
        if (role == null) throw new EntityNotFoundException("Роль не найдена");
        if (user == currentUser) throw new AccessDeniedException("Вы не можете изменить себе роль");
        var result = await userManager.RemoveFromRoleAsync(user, request.Role);
        if (!result.Succeeded) throw new DataException($"Ошибка: {string.Join(", ", result.Errors)}");
        return mapper.Map<UserDto>(user);
    }

    public async Task<IList<Claim>> AddClaimToRole(Claim claim, Role role)
    {
        await RequireClaimAsync(SystemClaims.CanChangeRole);

        var claimsExists = await roleManager.GetClaimsAsync(role);
        if (claimsExists.Any(claimExist => claimExist.Value == claim.Value))
            throw new ArgumentException("Роли уже присвоено это утверждение");
        var roleEntity = await roleManager.FindByNameAsync(role.Name);
        if (roleEntity == null) throw new EntityNotFoundException("Роль не найдена");
        await roleManager.AddClaimAsync(roleEntity, claim);
        return await roleManager.GetClaimsAsync(roleEntity);
    }

    public async Task<IList<Claim>> RemoveClaimFromRole(Claim claim, Role role)
    {
        await RequireClaimAsync(SystemClaims.CanChangeRole);

        var claimsExists = await roleManager.GetClaimsAsync(role);
        if (claimsExists.All(claimExist => claimExist.Type != claim.Type))
            throw new ArgumentException("Роли уже присвоено это утверждение");
        var roleEntity = await roleManager.FindByNameAsync(role.Name);
        if (roleEntity == null) throw new EntityNotFoundException("Роль не найдена");
        await roleManager.AddClaimAsync(roleEntity, claim);
        return await roleManager.GetClaimsAsync(roleEntity);
    }

    public async Task<bool> RequireClaimAsync(Claim claim)
    {
        var user = await GetCurrentUserAsync();
        var roles = await userManager.GetRolesAsync(user);
        foreach (var roleString in roles)
        {
            var role = await roleManager.FindByNameAsync(roleString);
            if (role == null) continue;
            if (role.Name == new Admin().Name) return true;
            var claims = await roleManager.GetClaimsAsync(role);
            if (claims.Any(cl => cl.Value == claim.Value)) return true;
        }

        throw new AccessDeniedException("Недостаточно прав для выполнения действия");
    }

    public async Task<bool> HasClaimAsync(Claim claim)
    {
        var user = await GetCurrentUserAsync();
        var roles = await userManager.GetRolesAsync(user);
        foreach (var roleString in roles)
        {
            var role = await roleManager.FindByNameAsync(roleString);
            if (role != null) continue;
            if (role.Name == new Admin().Name) return true;
            var claims = await roleManager.GetClaimsAsync(role);
            if (claims.Any(cl => cl == claim)) return true;
        }

        return false;
    }

    public async Task<List<RoleDto>> GetRolesAsync()
    {
        var roles = await roleManager.Roles.ToListAsync();
        var c = await roleManager.GetClaimsAsync(roles[0]);
        var rolesDto = mapper.Map<List<RoleDto>>(roles);
        foreach (var roleDto in rolesDto)
        {
            var roleEntity = await roleManager.FindByNameAsync(roleDto.Name);
            var claims = mapper.Map<List<ClaimDto>>(await roleManager.GetClaimsAsync(roleEntity));
            roleDto.Claims = claims;
        }

        return rolesDto;
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