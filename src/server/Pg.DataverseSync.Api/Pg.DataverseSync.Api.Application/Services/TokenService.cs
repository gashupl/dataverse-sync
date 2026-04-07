using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Pg.DataverseSync.Api.Application.Services.Interfaces;
using Pg.DataverseSync.Api.Application.Model;
using Pg.DataverseSync.Api.Application.Repositories;

namespace Pg.DataverseSync.Api.Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ITokenRepository _tokenRepository;

    public TokenService(IConfiguration configuration, ITokenRepository tokenRepository)
    {
        _configuration = configuration;
        _tokenRepository = tokenRepository;
    }

    public string GenerateJwtToken(int userId, string username, string email)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "10");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
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

    public async Task<RefreshToken> StoreRefreshTokenAsync(int userId, string token, int expirationMinutes)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            CreatedAt = DateTime.UtcNow
        };

        return await _tokenRepository.CreateRefreshTokenAsync(refreshToken);
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await _tokenRepository.GetRefreshTokenByTokenAsync(token);

        if (refreshToken is null)
            return null;

        if (refreshToken.RevokedAt is not null)
            return null;

        if (refreshToken.ExpiresAt < DateTime.UtcNow)
            return null;

        return refreshToken;
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        await _tokenRepository.RevokeTokenAsync(token);
    }

    public async Task RevokeAllUserRefreshTokensAsync(int userId)
    {
        await _tokenRepository.RevokeAllUserTokensAsync(userId);
    }
}
