using System.Data;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderFlow.Identity.Config;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.DTOs;
using OrderFlow.Shared.Models.Identity.Roles;

namespace OrderFlow.Identity.Services;

public class UserService(
    IMapper mapper,
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IRefreshTokensRepository refreshTokensRepository,
    IOptions<IdentityConfig> identityConfig,
    ICurrentUserService currentUserService
) : IUserService
{
    public async Task<UserDto> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await currentUserService.GetCurrentUserAsync();

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

        var currentUser = await currentUserService.GetCurrentUserAsync();
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

        var currentUser = await currentUserService.GetCurrentUserAsync();
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
        var user = await currentUserService.GetCurrentUserAsync();
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
        var user = await currentUserService.GetCurrentUserAsync();
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


}