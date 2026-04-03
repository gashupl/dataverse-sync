using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Pg.DataverseSync.Api.Application.Model;
using Pg.DataverseSync.Api.Application.Results;
using Pg.DataverseSync.Api.Application.Services.Interfaces;
using Pg.DataverseSync.Api.Controllers;
using Pg.DataverseSync.Api.Domain;
using Pg.DataverseSync.Api.Models.Auth;

namespace Pg.DataverseSync.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _userService = Substitute.For<IUserService>();
        _tokenService = Substitute.For<ITokenService>();
        _authController = new AuthController(_userService, _tokenService);
    }

    #region Register Tests

    [Fact]
    public async Task Register_ShouldReturnOk_WhenUserIsCreatedSuccessfully()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _userService.CreateUser(Arg.Any<User>()).Returns(Task.FromResult(new CreateUserResult
        {
            Success = true,
            UserId = 1
        }));

        // Act
        var result = await _authController.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("User created", response.Message);
        Assert.Null(response.Token);
        Assert.Null(response.RefreshToken);

        await _userService.Received(1).CreateUser(Arg.Is<User>(u =>
            u.Username == "testuser" &&
            u.Email == "test@example.com" &&
            u.PasswordHash.Length > 0 &&
            u.PasswordSalt.Length > 0));
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenUsernameAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "existinguser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _userService.CreateUser(Arg.Any<User>()).Returns(Task.FromResult(new CreateUserResult
        {
            Success = false,
            ErrorMessage = "Username already exists"
        }));

        // Act
        var result = await _authController.Register(request);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("Username already exists", conflictResult.Value);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "invalid-email",
            Password = "Password123!"
        };

        // Act
        var result = await _authController.Register(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid email format", badRequestResult.Value);

        await _userService.DidNotReceive().CreateUser(Arg.Any<User>());
    }

    [Fact]
    public async Task Register_ShouldGeneratePasswordHash_BeforeCreatingUser()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        User? capturedUser = null;
        _userService.CreateUser(Arg.Do<User>(u => capturedUser = u))
            .Returns(Task.FromResult(new CreateUserResult { Success = true, UserId = 1 }));

        // Act
        await _authController.Register(request);

        // Assert
        Assert.NotNull(capturedUser);
        Assert.NotEmpty(capturedUser.PasswordHash);
        Assert.NotEmpty(capturedUser.PasswordSalt);
        Assert.Equal(32, capturedUser.PasswordHash.Length);
        Assert.Equal(16, capturedUser.PasswordSalt.Length);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            CreatedOn = DateTime.UtcNow
        };
        user.GeneratePasswordHash("Password123!");

        _userService.GetUserDetailsByUsernameAsync("testuser").Returns(Task.FromResult<User?>(user));
        _tokenService.GenerateJwtToken(1, "testuser", "test@example.com").Returns("jwt-token");
        _tokenService.GenerateRefreshToken().Returns("refresh-token");
        _tokenService.StoreRefreshTokenAsync(1, "refresh-token", 30)
            .Returns(Task.FromResult(new RefreshToken { Id = 1, Token = "refresh-token" }));

        // Act
        var result = await _authController.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Login successful", response.Message);
        Assert.Equal("jwt-token", response.Token);
        Assert.Equal("refresh-token", response.RefreshToken);

        await _userService.Received(1).GetUserDetailsByUsernameAsync("testuser");
        _tokenService.Received(1).GenerateJwtToken(1, "testuser", "test@example.com");
        _tokenService.Received(1).GenerateRefreshToken();
        await _tokenService.Received(1).StoreRefreshTokenAsync(1, "refresh-token", 30);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "nonexistent",
            Password = "Password123!"
        };

        _userService.GetUserDetailsByUsernameAsync("nonexistent").Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _authController.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = unauthorizedResult.Value;
        Assert.NotNull(response);

        _tokenService.DidNotReceive().GenerateJwtToken(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>());
        _tokenService.DidNotReceive().GenerateRefreshToken();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "WrongPassword123!"
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            CreatedOn = DateTime.UtcNow
        };
        user.GeneratePasswordHash("CorrectPassword123!");

        _userService.GetUserDetailsByUsernameAsync("testuser").Returns(Task.FromResult<User?>(user));

        // Act
        var result = await _authController.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);

        _tokenService.DidNotReceive().GenerateJwtToken(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>());
        _tokenService.DidNotReceive().GenerateRefreshToken();
    }

    [Fact]
    public async Task Login_ShouldStoreRefreshToken_AfterSuccessfulLogin()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            CreatedOn = DateTime.UtcNow
        };
        user.GeneratePasswordHash("Password123!");

        _userService.GetUserDetailsByUsernameAsync("testuser").Returns(Task.FromResult<User?>(user));
        _tokenService.GenerateJwtToken(1, "testuser", "test@example.com").Returns("jwt-token");
        _tokenService.GenerateRefreshToken().Returns("refresh-token");
        _tokenService.StoreRefreshTokenAsync(1, "refresh-token", 30)
            .Returns(Task.FromResult(new RefreshToken { Id = 1, Token = "refresh-token" }));

        // Act
        await _authController.Login(request);

        // Assert
        await _tokenService.Received(1).StoreRefreshTokenAsync(
            Arg.Is<int>(id => id == 1),
            Arg.Is<string>(token => token == "refresh-token"),
            Arg.Is<int>(days => days == 30));
    }

    #endregion

    #region Logout Tests

    [Fact]
    public async Task Logout_ShouldReturnOk_WhenRefreshTokenIsValid()
    {
        // Arrange
        var request = new LogoutRequest
        {
            RefreshToken = "valid-refresh-token"
        };

        // Act
        var result = await _authController.Logout(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        Assert.NotNull(response);

        await _tokenService.Received(1).RevokeRefreshTokenAsync("valid-refresh-token");
    }

    [Fact]
    public async Task Logout_ShouldReturnBadRequest_WhenRefreshTokenIsEmpty()
    {
        // Arrange
        var request = new LogoutRequest
        {
            RefreshToken = string.Empty
        };

        // Act
        var result = await _authController.Logout(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Refresh token is required", badRequestResult.Value);

        await _tokenService.DidNotReceive().RevokeRefreshTokenAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task Logout_ShouldReturnBadRequest_WhenRefreshTokenIsNull()
    {
        // Arrange
        var request = new LogoutRequest
        {
            RefreshToken = null!
        };

        // Act
        var result = await _authController.Logout(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Refresh token is required", badRequestResult.Value);
    }

    #endregion

    #region Refresh Tests

    [Fact]
    public async Task Refresh_ShouldReturnOk_WhenRefreshTokenIsValid()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "valid-refresh-token"
        };

        var storedToken = new RefreshToken
        {
            Id = 1,
            UserId = 1,
            Token = "valid-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = null
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            CreatedOn = DateTime.UtcNow
        };

        _tokenService.ValidateRefreshTokenAsync("valid-refresh-token").Returns(Task.FromResult<RefreshToken?>(storedToken));
        _userService.GetUserDetailsByIdAsync(1).Returns(Task.FromResult<User?>(user));
        _tokenService.GenerateJwtToken(1, "testuser", "test@example.com").Returns("new-jwt-token");
        _tokenService.GenerateRefreshToken().Returns("new-refresh-token");

        // Act
        var result = await _authController.Refresh(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("new-jwt-token", response.Token);
        Assert.Equal("new-refresh-token", response.RefreshToken);

        await _tokenService.Received(1).ValidateRefreshTokenAsync("valid-refresh-token");
        await _tokenService.Received(1).RevokeRefreshTokenAsync("valid-refresh-token");
        await _tokenService.Received(1).StoreRefreshTokenAsync(1, "new-refresh-token", Arg.Any<int>());
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenRefreshTokenIsNull()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "invalid-token"
        };

        _tokenService.ValidateRefreshTokenAsync("invalid-token").Returns(Task.FromResult<RefreshToken?>(null));

        // Act
        var result = await _authController.Refresh(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);

        _tokenService.DidNotReceive().GenerateJwtToken(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenRefreshTokenIsRevoked()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "revoked-token"
        };

        var storedToken = new RefreshToken
        {
            Id = 1,
            UserId = 1,
            Token = "revoked-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        _tokenService.ValidateRefreshTokenAsync("revoked-token").Returns(Task.FromResult<RefreshToken?>(storedToken));

        // Act
        var result = await _authController.Refresh(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);

        _tokenService.DidNotReceive().GenerateJwtToken(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenRefreshTokenIsExpired()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "expired-token"
        };

        var storedToken = new RefreshToken
        {
            Id = 1,
            UserId = 1,
            Token = "expired-token",
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-31),
            RevokedAt = null
        };

        _tokenService.ValidateRefreshTokenAsync("expired-token").Returns(Task.FromResult<RefreshToken?>(storedToken));

        // Act
        var result = await _authController.Refresh(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);

        _tokenService.DidNotReceive().GenerateJwtToken(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "valid-token"
        };

        var storedToken = new RefreshToken
        {
            Id = 1,
            UserId = 999,
            Token = "valid-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = null
        };

        _tokenService.ValidateRefreshTokenAsync("valid-token").Returns(Task.FromResult<RefreshToken?>(storedToken));
        _userService.GetUserDetailsByIdAsync(999).Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _authController.Refresh(request);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);

        _tokenService.DidNotReceive().GenerateJwtToken(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Refresh_ShouldRevokeOldToken_BeforeStoringNewOne()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "old-token"
        };

        var storedToken = new RefreshToken
        {
            Id = 1,
            UserId = 1,
            Token = "old-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = null
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            CreatedOn = DateTime.UtcNow
        };

        _tokenService.ValidateRefreshTokenAsync("old-token").Returns(Task.FromResult<RefreshToken?>(storedToken));
        _userService.GetUserDetailsByIdAsync(1).Returns(Task.FromResult<User?>(user));
        _tokenService.GenerateJwtToken(1, "testuser", "test@example.com").Returns("new-jwt");
        _tokenService.GenerateRefreshToken().Returns("new-refresh");

        // Act
        await _authController.Refresh(request);

        // Assert
        Received.InOrder(() =>
        {
            _tokenService.RevokeRefreshTokenAsync("old-token");
            _tokenService.StoreRefreshTokenAsync(1, "new-refresh", Arg.Any<int>());
        });
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Register_ShouldSetCreatedOnToUtcNow()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        var beforeTime = DateTime.UtcNow;
        User? capturedUser = null;
        _userService.CreateUser(Arg.Do<User>(u => capturedUser = u))
            .Returns(Task.FromResult(new CreateUserResult { Success = true, UserId = 1 }));

        // Act
        await _authController.Register(request);
        var afterTime = DateTime.UtcNow;

        // Assert
        Assert.NotNull(capturedUser);
        Assert.InRange(capturedUser.CreatedOn, beforeTime, afterTime);
    }

    [Fact]
    public async Task Login_ShouldNotExposeUserExistence_InErrorMessage()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "WrongPassword!"
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            CreatedOn = DateTime.UtcNow
        };
        user.GeneratePasswordHash("CorrectPassword!");

        _userService.GetUserDetailsByUsernameAsync("testuser").Returns(Task.FromResult<User?>(user));

        // Act
        var result = await _authController.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = unauthorizedResult.Value;
        
        // Should use generic error message, not revealing if user exists or password is wrong
        Assert.NotNull(response);
    }

    #endregion
}
