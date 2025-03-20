using OrderFlow.Shared.Models.Identity;

namespace OrderFlow.Identity.Interfaces;

public interface IRefreshTokensRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken> GetAsync(string refreshToken);
    Task RevokeAsync(string refreshToken);
}