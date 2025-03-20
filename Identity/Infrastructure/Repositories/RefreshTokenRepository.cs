using OrderFlow.Identity.Interfaces;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Models.Identity;

namespace OrderFlow.Identity.Infrastructure.Repositories;

public class RefreshTokenRepository(DataContext context) : IRefreshTokensRepository
{
    public async Task AddAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken> GetAsync(string refreshToken)
    {
        var token = context.RefreshTokens.SingleOrDefault(t =>
            t.Token == refreshToken &&
            t.ExpiryDate > DateTime.UtcNow.AddDays(-15) &&
            t.IsRevoked == false);
        if (token == null) throw new UnauthorizedAccessException();
        return token;
    }

    public async Task RevokeAsync(string refreshToken)
    {
        var token = context.RefreshTokens.SingleOrDefault(t =>
            t.Token == refreshToken &&
            t.ExpiryDate > DateTime.UtcNow.AddDays(-15) &&
            t.IsRevoked == false);
        if (token == null) throw new UnauthorizedAccessException();
        token.IsRevoked = true;
        await context.SaveChangesAsync();
    }
}