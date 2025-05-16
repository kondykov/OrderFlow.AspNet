using System.Security.Claims;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.DTOs;

namespace OrderFlow.Identity.Interfaces;

public interface IUserService
{
    Task<UserDto> ChangePasswordAsync(ChangePasswordRequest request);
    Task<UserDto> AddRoleAsync(ChangeRoleRequest request);
    Task<UserDto> RemoveRoleAsync(ChangeRoleRequest request);
    Task<IList<Claim>> AddClaimToRole(Claim claim, Role role);
    Task<IList<Claim>> RemoveClaimFromRole(Claim claim, Role role);
    Task<bool> RequireClaimAsync(Claim claim);
    Task<bool> HasClaimAsync(Claim claim);
    Task<List<RoleDto>> GetRolesAsync();
}