using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.DTOs;
using LoginRequest = OrderFlow.Identity.Models.Request.LoginRequest;
using RegisterRequest = OrderFlow.Identity.Models.Request.RegisterRequest;

namespace OrderFlow.Identity.Interfaces;

public interface IUserService
{
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<AuthenticationResponse> RefreshTokenAsync(RefreshRequest request);
    Task<User> GetCurrentUserAsync();
    Task<UserDto> GetCurrentUserInfoAsync();
    Task<UserDto> ChangePasswordAsync(ChangePasswordRequest request);
    Task<UserDto> AddRoleAsync(ChangeRoleRequest request);
    Task<UserDto> RemoveRoleAsync(ChangeRoleRequest request);
    Task<IList<Claim>> AddClaimToRole(Claim claim, Role role);
    Task<IList<Claim>> RemoveClaimFromRole(Claim claim, Role role);
    Task<bool> RequireClaimAsync(Claim claim);
    Task<bool> HasClaimAsync(Claim claim);
    Task<List<RoleDto>> GetRolesAsync();
}