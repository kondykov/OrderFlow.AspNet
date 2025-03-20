using OrderFlow.Identity.Models;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.DTOs;

namespace OrderFlow.Identity.Interfaces;

public interface IUserService
{
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<User> GetCurrentUserAsync();
    
    Task<UserDto> ChangePasswordAsync(ChangePasswordRequest request);
    Task<UserDto> AddRoleAsync(ChangeRoleRequest request);
    Task<UserDto> RemoveRoleAsync(ChangeRoleRequest request);
}