namespace Pg.DataverseSync.Api.Application.Services.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(int userId, string username, string email);

    string GenerateRefreshToken();
}
