using Pg.DataverseSync.Api.Application.Model;

namespace Pg.DataverseSync.Api.Application.Repositories;

public interface ITokenRepository
{
    Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetUserRefreshTokensAsync(int userId);
    Task RevokeTokenAsync(string token);
    Task RevokeAllUserTokensAsync(int userId);
}
