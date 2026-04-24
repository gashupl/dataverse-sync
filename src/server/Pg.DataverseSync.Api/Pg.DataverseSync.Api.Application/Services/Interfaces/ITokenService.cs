using Pg.DataverseSync.Api.Application.Model;

namespace Pg.DataverseSync.Api.Application.Services.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(int userId, string username, string email);
    string GenerateRefreshToken();
    Task<RefreshToken> StoreRefreshTokenAsync(int userId, string token, int expirationMinutes);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string token);
    Task RevokeAllUserRefreshTokensAsync(int userId);
}
