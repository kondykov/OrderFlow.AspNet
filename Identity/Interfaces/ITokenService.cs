using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.Devices;

namespace OrderFlow.Identity.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateAccessToken(Device device);
    Task<RefreshToken> GenerateRefreshToken(User user);
}