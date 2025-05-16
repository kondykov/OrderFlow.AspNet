using Microsoft.AspNetCore.Identity.Data;
using OrderFlow.Identity.Models.Response;
using LoginRequest = OrderFlow.Identity.Models.Request.LoginRequest;
using RegisterRequest = OrderFlow.Identity.Models.Request.RegisterRequest;

namespace OrderFlow.Identity.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<AuthenticationResponse> RefreshTokenAsync(RefreshRequest request);
}