using Pg.DataverseSync.Api.Application.Model;
using Pg.DataverseSync.Api.Domain;
using Pg.DataverseSync.Api.Infrastructure.Data;
using Pg.DataverseSync.Api.Infrastructure.Repositories;
using Pg.DataverseSync.Api.Infrastructure.Tests.Helpers;

namespace Pg.DataverseSync.Api.Infrastructure.Tests.Repositories;

public class TokenRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly TokenRepository _tokenRepository;
    private readonly UserRepository _userRepository;

    public TokenRepositoryTests()
    {
        _dbContext = DbContextFactory.CreateInMemoryDbContext(Guid.NewGuid().ToString());
        _tokenRepository = new TokenRepository(_dbContext);
        _userRepository = new UserRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    private async Task<User> CreateTestUser(string username = "testuser")
    {
        var user = new User
        {
            Username = username,
            Email = $"{username}@example.com",
            PasswordHash = new byte[] { 1, 2, 3 },
            PasswordSalt = new byte[] { 4, 5, 6 },
            CreatedOn = DateTime.UtcNow
        };
        await _userRepository.CreateUser(user);
        return user;
    }

    [Fact]
    public async Task CreateRefreshTokenAsync_ShouldAddTokenToDatabase_AndReturnToken()
    {
        // Arrange
        var user = await CreateTestUser();
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "test-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var createdToken = await _tokenRepository.CreateRefreshTokenAsync(refreshToken);

        // Assert
        Assert.NotNull(createdToken);
        Assert.True(createdToken.Id > 0);
        Assert.Equal(user.Id, createdToken.UserId);
        Assert.Equal("test-refresh-token", createdToken.Token);

        var savedToken = await _dbContext.RefreshTokens.FindAsync(createdToken.Id);
        Assert.NotNull(savedToken);
        Assert.Equal("test-refresh-token", savedToken.Token);
    }

    [Fact]
    public async Task GetRefreshTokenByTokenAsync_ShouldReturnToken_WhenTokenExists()
    {
        // Arrange
        var user = await CreateTestUser();
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "test-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(refreshToken);

        // Act
        var foundToken = await _tokenRepository.GetRefreshTokenByTokenAsync("test-refresh-token");

        // Assert
        Assert.NotNull(foundToken);
        Assert.Equal(user.Id, foundToken.UserId);
        Assert.Equal("test-refresh-token", foundToken.Token);
    }

    [Fact]
    public async Task GetRefreshTokenByTokenAsync_ShouldReturnNull_WhenTokenDoesNotExist()
    {
        // Act
        var foundToken = await _tokenRepository.GetRefreshTokenByTokenAsync("nonexistent-token");

        // Assert
        Assert.Null(foundToken);
    }

    [Fact]
    public async Task GetUserRefreshTokensAsync_ShouldReturnOnlyActiveTokens()
    {
        // Arrange
        var user = await CreateTestUser();

        var activeToken1 = new RefreshToken
        {
            UserId = user.Id,
            Token = "active-token-1",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(activeToken1);

        var activeToken2 = new RefreshToken
        {
            UserId = user.Id,
            Token = "active-token-2",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(activeToken2);

        var revokedToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "revoked-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(revokedToken);

        // Act
        var tokens = await _tokenRepository.GetUserRefreshTokensAsync(user.Id);

        // Assert
        var tokenList = tokens.ToList();
        Assert.Equal(2, tokenList.Count);
        Assert.Contains(tokenList, t => t.Token == "active-token-1");
        Assert.Contains(tokenList, t => t.Token == "active-token-2");
        Assert.DoesNotContain(tokenList, t => t.Token == "revoked-token");
    }

    [Fact]
    public async Task GetUserRefreshTokensAsync_ShouldReturnEmpty_WhenNoActiveTokens()
    {
        // Arrange
        var user = await CreateTestUser();

        // Act
        var tokens = await _tokenRepository.GetUserRefreshTokensAsync(user.Id);

        // Assert
        Assert.Empty(tokens);
    }

    [Fact]
    public async Task RevokeTokenAsync_ShouldSetRevokedAt_WhenTokenExists()
    {
        // Arrange
        var user = await CreateTestUser();
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "test-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(refreshToken);

        // Act
        await _tokenRepository.RevokeTokenAsync("test-refresh-token");

        // Assert
        var revokedToken = await _tokenRepository.GetRefreshTokenByTokenAsync("test-refresh-token");
        Assert.NotNull(revokedToken);
        Assert.NotNull(revokedToken.RevokedAt);
        Assert.True(revokedToken.RevokedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task RevokeTokenAsync_ShouldDoNothing_WhenTokenDoesNotExist()
    {
        // Act & Assert - Should not throw exception
        await _tokenRepository.RevokeTokenAsync("nonexistent-token");
    }

    [Fact]
    public async Task RevokeAllUserTokensAsync_ShouldRevokeAllActiveTokensForUser()
    {
        // Arrange
        var user1 = await CreateTestUser("user1");
        var user2 = await CreateTestUser("user2");

        var user1Token1 = new RefreshToken
        {
            UserId = user1.Id,
            Token = "user1-token-1",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(user1Token1);

        var user1Token2 = new RefreshToken
        {
            UserId = user1.Id,
            Token = "user1-token-2",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(user1Token2);

        var user2Token = new RefreshToken
        {
            UserId = user2.Id,
            Token = "user2-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(user2Token);

        // Act
        await _tokenRepository.RevokeAllUserTokensAsync(user1.Id);

        // Assert
        var user1Tokens = await _tokenRepository.GetUserRefreshTokensAsync(user1.Id);
        Assert.Empty(user1Tokens);

        var user1Token1Revoked = await _tokenRepository.GetRefreshTokenByTokenAsync("user1-token-1");
        Assert.NotNull(user1Token1Revoked?.RevokedAt);

        var user1Token2Revoked = await _tokenRepository.GetRefreshTokenByTokenAsync("user1-token-2");
        Assert.NotNull(user1Token2Revoked?.RevokedAt);

        var user2Tokens = await _tokenRepository.GetUserRefreshTokensAsync(user2.Id);
        Assert.Single(user2Tokens);
    }

    [Fact]
    public async Task RevokeAllUserTokensAsync_ShouldNotRevokeAlreadyRevokedTokens()
    {
        // Arrange
        var user = await CreateTestUser();

        var alreadyRevokedToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "already-revoked",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = DateTime.UtcNow.AddMinutes(-10)
        };
        await _tokenRepository.CreateRefreshTokenAsync(alreadyRevokedToken);

        var activeToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "active-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(activeToken);

        var originalRevokedAt = alreadyRevokedToken.RevokedAt;

        // Act
        await _tokenRepository.RevokeAllUserTokensAsync(user.Id);

        // Assert
        var alreadyRevokedCheck = await _tokenRepository.GetRefreshTokenByTokenAsync("already-revoked");
        Assert.Equal(originalRevokedAt, alreadyRevokedCheck?.RevokedAt);

        var activeTokenCheck = await _tokenRepository.GetRefreshTokenByTokenAsync("active-token");
        Assert.NotNull(activeTokenCheck?.RevokedAt);
    }

    [Fact]
    public async Task CreateRefreshTokenAsync_ShouldStoreExpiresAtAndCreatedAt()
    {
        // Arrange
        var user = await CreateTestUser();
        var expiresAt = DateTime.UtcNow.AddDays(30);
        var createdAt = DateTime.UtcNow;

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "test-refresh-token",
            ExpiresAt = expiresAt,
            CreatedAt = createdAt
        };

        // Act
        var createdToken = await _tokenRepository.CreateRefreshTokenAsync(refreshToken);

        // Assert
        Assert.Equal(expiresAt, createdToken.ExpiresAt, TimeSpan.FromSeconds(1));
        Assert.Equal(createdAt, createdToken.CreatedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetUserRefreshTokensAsync_ShouldReturnOnlyTokensForSpecificUser()
    {
        // Arrange
        var user1 = await CreateTestUser("user1");
        var user2 = await CreateTestUser("user2");

        var user1Token = new RefreshToken
        {
            UserId = user1.Id,
            Token = "user1-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(user1Token);

        var user2Token = new RefreshToken
        {
            UserId = user2.Id,
            Token = "user2-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        await _tokenRepository.CreateRefreshTokenAsync(user2Token);

        // Act
        var user1Tokens = await _tokenRepository.GetUserRefreshTokensAsync(user1.Id);
        var user2Tokens = await _tokenRepository.GetUserRefreshTokensAsync(user2.Id);

        // Assert
        Assert.Single(user1Tokens);
        Assert.Equal("user1-token", user1Tokens.First().Token);

        Assert.Single(user2Tokens);
        Assert.Equal("user2-token", user2Tokens.First().Token);
    }
}
