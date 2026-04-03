using Microsoft.Extensions.Configuration;
using NSubstitute;
using Pg.DataverseSync.Api.Application.Model;
using Pg.DataverseSync.Api.Application.Repositories;
using Pg.DataverseSync.Api.Application.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Pg.DataverseSync.Api.Application.Tests.Services;

public class TokenServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly ITokenRepository _tokenRepository;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _configuration = Substitute.For<IConfiguration>();
        _tokenRepository = Substitute.For<ITokenRepository>();

        // Setup configuration mock
        var jwtSection = Substitute.For<IConfigurationSection>();
        jwtSection["SecretKey"].Returns("your-super-secret-key-change-this-in-production-min-32-chars!!");
        jwtSection["Issuer"].Returns("https://localhost:7116");
        jwtSection["Audience"].Returns("Pg.DataverseSync.Api");
        jwtSection["ExpirationMinutes"].Returns("10");

        _configuration.GetSection("Jwt").Returns(jwtSection);

        _tokenService = new TokenService(_configuration, _tokenRepository);
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnValidToken()
    {
        // Arrange
        var userId = 1;
        var username = "testuser";
        var email = "test@example.com";

        // Act
        var token = _tokenService.GenerateJwtToken(userId, username, email);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Verify token structure
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("https://localhost:7116", jwtToken.Issuer);
        Assert.Contains("Pg.DataverseSync.Api", jwtToken.Audiences);
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && c.Value == "1");
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" && c.Value == "testuser");
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && c.Value == "test@example.com");
    }

    [Fact]
    public void GenerateJwtToken_ShouldHaveCorrectExpiration()
    {
        // Arrange
        var userId = 1;
        var username = "testuser";
        var email = "test@example.com";

        // Act
        var token = _tokenService.GenerateJwtToken(userId, username, email);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var expectedExpiration = DateTime.UtcNow.AddMinutes(10);
        Assert.True(jwtToken.ValidTo >= expectedExpiration.AddSeconds(-5));
        Assert.True(jwtToken.ValidTo <= expectedExpiration.AddSeconds(5));
    }

    [Fact]
    public void GenerateJwtToken_ShouldCreateDifferentTokensForDifferentUsers()
    {
        // Arrange & Act
        var token1 = _tokenService.GenerateJwtToken(1, "user1", "user1@example.com");
        var token2 = _tokenService.GenerateJwtToken(2, "user2", "user2@example.com");

        // Assert
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnNonEmptyToken()
    {
        // Act
        var token = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnBase64String()
    {
        // Act
        var token = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(token);
        
        // Verify it's valid Base64
        var bytes = Convert.FromBase64String(token);
        Assert.NotEmpty(bytes);
        Assert.Equal(32, bytes.Length);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldCreateDifferentTokens()
    {
        // Act
        var token1 = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public async Task StoreRefreshTokenAsync_ShouldCallRepositoryWithCorrectData()
    {
        // Arrange
        var userId = 1;
        var token = "test-refresh-token";
        var expirationDays = 30;

        var expectedRefreshToken = new RefreshToken
        {
            Id = 1,
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
            CreatedAt = DateTime.UtcNow
        };

        _tokenRepository.CreateRefreshTokenAsync(Arg.Any<RefreshToken>())
            .Returns(Task.FromResult(expectedRefreshToken));

        // Act
        var result = await _tokenService.StoreRefreshTokenAsync(userId, token, expirationDays);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(token, result.Token);

        await _tokenRepository.Received(1).CreateRefreshTokenAsync(Arg.Is<RefreshToken>(rt =>
            rt.UserId == userId &&
            rt.Token == token &&
            rt.ExpiresAt > DateTime.UtcNow &&
            rt.CreatedAt <= DateTime.UtcNow));
    }

    [Fact]
    public async Task StoreRefreshTokenAsync_ShouldUseDefaultExpirationWhenNotProvided()
    {
        // Arrange
        var userId = 1;
        var token = "test-refresh-token";

        var expectedRefreshToken = new RefreshToken
        {
            Id = 1,
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };

        _tokenRepository.CreateRefreshTokenAsync(Arg.Any<RefreshToken>())
            .Returns(Task.FromResult(expectedRefreshToken));

        // Act
        var result = await _tokenService.StoreRefreshTokenAsync(userId, token);

        // Assert
        await _tokenRepository.Received(1).CreateRefreshTokenAsync(Arg.Is<RefreshToken>(rt =>
            rt.ExpiresAt >= DateTime.UtcNow.AddDays(29) &&
            rt.ExpiresAt <= DateTime.UtcNow.AddDays(31)));
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_ShouldReturnToken_WhenTokenIsValid()
    {
        // Arrange
        var token = "valid-token";
        var refreshToken = new RefreshToken
        {
            Id = 1,
            UserId = 1,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = null
        };

        _tokenRepository.GetRefreshTokenByTokenAsync(token).Returns(Task.FromResult<RefreshToken?>(refreshToken));

        // Act
        var result = await _tokenService.ValidateRefreshTokenAsync(token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(token, result.Token);
        Assert.Null(result.RevokedAt);

        await _tokenRepository.Received(1).GetRefreshTokenByTokenAsync(token);
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_ShouldReturnNull_WhenTokenDoesNotExist()
    {
        // Arrange
        var token = "nonexistent-token";
        _tokenRepository.GetRefreshTokenByTokenAsync(token).Returns(Task.FromResult<RefreshToken?>(null));

        // Act
        var result = await _tokenService.ValidateRefreshTokenAsync(token);

        // Assert
        Assert.Null(result);

        await _tokenRepository.Received(1).GetRefreshTokenByTokenAsync(token);
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_ShouldReturnNull_WhenTokenIsRevoked()
    {
        // Arrange
        var token = "revoked-token";
        var refreshToken = new RefreshToken
        {
            Id = 1,
            UserId = 1,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        _tokenRepository.GetRefreshTokenByTokenAsync(token).Returns(Task.FromResult<RefreshToken?>(refreshToken));

        // Act
        var result = await _tokenService.ValidateRefreshTokenAsync(token);

        // Assert
        Assert.Null(result);

        await _tokenRepository.Received(1).GetRefreshTokenByTokenAsync(token);
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_ShouldReturnNull_WhenTokenIsExpired()
    {
        // Arrange
        var token = "expired-token";
        var refreshToken = new RefreshToken
        {
            Id = 1,
            UserId = 1,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-31),
            RevokedAt = null
        };

        _tokenRepository.GetRefreshTokenByTokenAsync(token).Returns(Task.FromResult<RefreshToken?>(refreshToken));

        // Act
        var result = await _tokenService.ValidateRefreshTokenAsync(token);

        // Assert
        Assert.Null(result);

        await _tokenRepository.Received(1).GetRefreshTokenByTokenAsync(token);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_ShouldCallRepository()
    {
        // Arrange
        var token = "token-to-revoke";

        // Act
        await _tokenService.RevokeRefreshTokenAsync(token);

        // Assert
        await _tokenRepository.Received(1).RevokeTokenAsync(token);
    }

    [Fact]
    public async Task RevokeAllUserRefreshTokensAsync_ShouldCallRepository()
    {
        // Arrange
        var userId = 1;

        // Act
        await _tokenService.RevokeAllUserRefreshTokensAsync(userId);

        // Assert
        await _tokenRepository.Received(1).RevokeAllUserTokensAsync(userId);
    }

    [Fact]
    public void GenerateJwtToken_ShouldUseConfigurationValues()
    {
        // Arrange
        var userId = 1;
        var username = "testuser";
        var email = "test@example.com";

        // Act
        var token = _tokenService.GenerateJwtToken(userId, username, email);

        // Assert
        _configuration.Received(1).GetSection("Jwt");

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("https://localhost:7116", jwtToken.Issuer);
        Assert.Contains("Pg.DataverseSync.Api", jwtToken.Audiences);
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_ShouldReturnNull_WhenTokenExpiresAtBoundary()
    {
        // Arrange
        var token = "boundary-token";
        var refreshToken = new RefreshToken
        {
            Id = 1,
            UserId = 1,
            Token = token,
            ExpiresAt = DateTime.UtcNow, // Expires right now
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            RevokedAt = null
        };

        _tokenRepository.GetRefreshTokenByTokenAsync(token).Returns(Task.FromResult<RefreshToken?>(refreshToken));

        // Act
        var result = await _tokenService.ValidateRefreshTokenAsync(token);

        // Assert
        // Token that expires exactly now should be considered expired
        Assert.Null(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(90)]
    public async Task StoreRefreshTokenAsync_ShouldAcceptVariousExpirationDays(int expirationDays)
    {
        // Arrange
        var userId = 1;
        var token = "test-token";

        var expectedRefreshToken = new RefreshToken
        {
            Id = 1,
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
            CreatedAt = DateTime.UtcNow
        };

        _tokenRepository.CreateRefreshTokenAsync(Arg.Any<RefreshToken>())
            .Returns(Task.FromResult(expectedRefreshToken));

        // Act
        var result = await _tokenService.StoreRefreshTokenAsync(userId, token, expirationDays);

        // Assert
        Assert.NotNull(result);
        await _tokenRepository.Received(1).CreateRefreshTokenAsync(Arg.Any<RefreshToken>());
    }
}
