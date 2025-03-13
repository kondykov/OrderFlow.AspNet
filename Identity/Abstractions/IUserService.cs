using OrderFlow.Identity.Models.Request;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Models;

namespace OrderFlow.Identity.Abstractions;

public interface IUserService
{
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<User> GetCurrentUserAsync();
}