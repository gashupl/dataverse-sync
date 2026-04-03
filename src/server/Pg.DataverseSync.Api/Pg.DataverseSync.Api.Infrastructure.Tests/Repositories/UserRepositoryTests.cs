using Pg.DataverseSync.Api.Domain;
using Pg.DataverseSync.Api.Infrastructure.Data;
using Pg.DataverseSync.Api.Infrastructure.Repositories;
using Pg.DataverseSync.Api.Infrastructure.Tests.Helpers;

namespace Pg.DataverseSync.Api.Infrastructure.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _dbContext = DbContextFactory.CreateInMemoryDbContext(Guid.NewGuid().ToString());
        _userRepository = new UserRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task CreateUser_ShouldAddUserToDatabase_AndReturnUserId()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            CreatedOn = DateTime.UtcNow
        };

        // Act
        var userId = await _userRepository.CreateUser(user);

        // Assert
        Assert.True(userId > 0);
        Assert.Equal(user.Id, userId);

        var savedUser = await _dbContext.Users.FindAsync(userId);
        Assert.NotNull(savedUser);
        Assert.Equal("testuser", savedUser.Username);
        Assert.Equal("test@example.com", savedUser.Email);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            CreatedOn = DateTime.UtcNow
        };
        await _userRepository.CreateUser(user);

        // Act
        var foundUser = await _userRepository.FindByIdAsync(user.Id);

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(user.Id, foundUser.Id);
        Assert.Equal("testuser", foundUser.Username);
        Assert.Equal("test@example.com", foundUser.Email);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var foundUser = await _userRepository.FindByIdAsync(999);

        // Assert
        Assert.Null(foundUser);
    }

    [Fact]
    public async Task FindByUsernameAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            CreatedOn = DateTime.UtcNow
        };
        await _userRepository.CreateUser(user);

        // Act
        var foundUser = await _userRepository.FindByUsernameAsync("testuser");

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(user.Id, foundUser.Id);
        Assert.Equal("testuser", foundUser.Username);
    }

    [Fact]
    public async Task FindByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var foundUser = await _userRepository.FindByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(foundUser);
    }

    [Fact]
    public async Task FindByEmailAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            CreatedOn = DateTime.UtcNow
        };
        await _userRepository.CreateUser(user);

        // Act
        var foundUser = await _userRepository.FindByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(user.Id, foundUser.Id);
        Assert.Equal("test@example.com", foundUser.Email);
    }

    [Fact]
    public async Task FindByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var foundUser = await _userRepository.FindByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(foundUser);
    }

    [Fact]
    public async Task CreateUser_ShouldStorePasswordHashAndSalt()
    {
        // Arrange
        var passwordHash = (byte[])[1, 2, 3, 4, 5];
        var passwordSalt = (byte[])[6, 7, 8, 9, 10];

        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            CreatedOn = DateTime.UtcNow
        };

        // Act
        var userId = await _userRepository.CreateUser(user);
        var savedUser = await _userRepository.FindByIdAsync(userId);

        // Assert
        Assert.NotNull(savedUser);
        Assert.Equal(passwordHash, savedUser.PasswordHash);
        Assert.Equal(passwordSalt, savedUser.PasswordSalt);
    }

    [Fact]
    public async Task CreateUser_ShouldPreserveCreatedOnTimestamp()
    {
        // Arrange
        var createdOn = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            CreatedOn = createdOn
        };

        // Act
        var userId = await _userRepository.CreateUser(user);
        var savedUser = await _userRepository.FindByIdAsync(userId);

        // Assert
        Assert.NotNull(savedUser);
        Assert.Equal(createdOn, savedUser.CreatedOn);
    }
}
