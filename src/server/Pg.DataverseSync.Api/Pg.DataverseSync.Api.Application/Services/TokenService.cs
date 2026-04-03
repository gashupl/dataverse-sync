using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Pg.DataverseSync.Api.Application.Services.Interfaces;
using Pg.DataverseSync.Api.Application.Model;

namespace Pg.DataverseSync.Api.Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(int userId, string username, string email)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "10");

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes);
    }

    public Task RevokeAllUserRefreshTokensAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task RevokeRefreshTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken> StoreRefreshTokenAsync(int userId, string token, int expirationDays = 30)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        throw new NotImplementedException();
    }
}
