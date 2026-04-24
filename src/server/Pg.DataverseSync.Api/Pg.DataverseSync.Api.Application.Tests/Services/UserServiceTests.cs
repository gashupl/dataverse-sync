using NSubstitute;
using Pg.DataverseSync.Api.Application.Repositories;
using Pg.DataverseSync.Api.Application.Services;
using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Application.Tests.Services;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _userService = new UserService(_userRepository);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnSuccess_WhenUserIsCreatedSuccessfully()
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

        _userRepository.FindByUsernameAsync(user.Username).Returns(Task.FromResult<User?>(null));
        _userRepository.FindByEmailAsync(user.Email).Returns(Task.FromResult<User?>(null));
        _userRepository.CreateUser(user).Returns(Task.FromResult(1));

        // Act
        var result = await _userService.CreateUser(user);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.UserId);
        Assert.Null(result.ErrorMessage);

        await _userRepository.Received(1).FindByUsernameAsync(user.Username);
        await _userRepository.Received(1).FindByEmailAsync(user.Email);
        await _userRepository.Received(1).CreateUser(user);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenUsernameAlreadyExists()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "existing@example.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            CreatedOn = DateTime.UtcNow
        };

        var newUser = new User
        {
            Username = "testuser",
            Email = "newemail@example.com",
            PasswordHash = [7, 8, 9],
            PasswordSalt = [10, 11, 12],
            CreatedOn = DateTime.UtcNow
        };

        _userRepository.FindByUsernameAsync(newUser.Username).Returns(Task.FromResult<User?>(existingUser));

        // Act
        var result = await _userService.CreateUser(newUser);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.UserId);
        Assert.Equal("Username already exists", result.ErrorMessage);

        await _userRepository.Received(1).FindByUsernameAsync(newUser.Username);
        await _userRepository.DidNotReceive().FindByEmailAsync(Arg.Any<string>());
        await _userRepository.DidNotReceive().CreateUser(Arg.Any<User>());
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenEmailAlreadyExists()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Username = "existinguser",
            Email = "test@example.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            CreatedOn = DateTime.UtcNow
        };

        var newUser = new User
        {
            Username = "newuser",
            Email = "test@example.com",
            PasswordHash = [7, 8, 9],
            PasswordSalt = [10, 11, 12],
            CreatedOn = DateTime.UtcNow
        };

        _userRepository.FindByUsernameAsync(newUser.Username).Returns(Task.FromResult<User?>(null));
        _userRepository.FindByEmailAsync(newUser.Email).Returns(Task.FromResult<User?>(existingUser));

        // Act
        var result = await _userService.CreateUser(newUser);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.UserId);
        Assert.Equal("Email already exists", result.ErrorMessage);

        await _userRepository.Received(1).FindByUsernameAsync(newUser.Username);
        await _userRepository.Received(1).FindByEmailAsync(newUser.Email);
        await _userRepository.DidNotReceive().CreateUser(Arg.Any<User>());
    }

    [Fact]
    public async Task GetUserDetailsByUsernameAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            CreatedOn = DateTime.UtcNow
        };

        _userRepository.FindByUsernameAsync("testuser").Returns(Task.FromResult<User?>(user));

        // Act
        var result = await _userService.GetUserDetailsByUsernameAsync("testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("test@example.com", result.Email);

        await _userRepository.Received(1).FindByUsernameAsync("testuser");
    }

    [Fact]
    public async Task GetUserDetailsByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepository.FindByUsernameAsync("nonexistent").Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _userService.GetUserDetailsByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);

        await _userRepository.Received(1).FindByUsernameAsync("nonexistent");
    }

    [Fact]
    public async Task GetUserDetailsByEmailAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            CreatedOn = DateTime.UtcNow
        };

        _userRepository.FindByEmailAsync("test@example.com").Returns(Task.FromResult<User?>(user));

        // Act
        var result = await _userService.GetUserDetailsByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("test@example.com", result.Email);

        await _userRepository.Received(1).FindByEmailAsync("test@example.com");
    }

    [Fact]
    public async Task GetUserDetailsByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepository.FindByEmailAsync("nonexistent@example.com").Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _userService.GetUserDetailsByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);

        await _userRepository.Received(1).FindByEmailAsync("nonexistent@example.com");
    }

    [Fact]
    public async Task GetUserDetailsByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            CreatedOn = DateTime.UtcNow
        };

        _userRepository.FindByIdAsync(1).Returns(Task.FromResult<User?>(user));

        // Act
        var result = await _userService.GetUserDetailsByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("test@example.com", result.Email);

        await _userRepository.Received(1).FindByIdAsync(1);
    }

    [Fact]
    public async Task GetUserDetailsByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepository.FindByIdAsync(999).Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _userService.GetUserDetailsByIdAsync(999);

        // Assert
        Assert.Null(result);

        await _userRepository.Received(1).FindByIdAsync(999);
    }

    [Fact]
    public async Task CreateUser_ShouldCheckUsernameBeforeEmail()
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

        var existingUser = new User { Id = 1, Username = "testuser" };
        _userRepository.FindByUsernameAsync(user.Username).Returns(Task.FromResult<User?>(existingUser));

        // Act
        var result = await _userService.CreateUser(user);

        // Assert
        Assert.False(result.Success);
        await _userRepository.Received(1).FindByUsernameAsync(user.Username);
        await _userRepository.DidNotReceive().FindByEmailAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task CreateUser_ShouldReturnUserIdFromRepository()
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

        _userRepository.FindByUsernameAsync(user.Username).Returns(Task.FromResult<User?>(null));
        _userRepository.FindByEmailAsync(user.Email).Returns(Task.FromResult<User?>(null));
        _userRepository.CreateUser(user).Returns(Task.FromResult(42));

        // Act
        var result = await _userService.CreateUser(user);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(42, result.UserId);
    }
}
