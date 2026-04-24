using Microsoft.EntityFrameworkCore;
using Pg.DataverseSync.Api.Application.Model;
using Pg.DataverseSync.Api.Application.Repositories;
using Pg.DataverseSync.Api.Infrastructure.Data;

namespace Pg.DataverseSync.Api.Infrastructure.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly AppDbContext _dbContext;

    public TokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IEnumerable<RefreshToken>> GetUserRefreshTokensAsync(int userId)
    {
        return await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();
    }

    public async Task RevokeTokenAsync(string token)
    {
        var refreshToken = await GetRefreshTokenByTokenAsync(token);
        if (refreshToken is not null)
        {
            refreshToken.RevokedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RevokeAllUserTokensAsync(int userId)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }
}
