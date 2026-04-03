using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pg.DataverseSync.Api.Application.Services.Interfaces;
using Pg.DataverseSync.Api.Domain;
using Pg.DataverseSync.Api.Models.Auth;

namespace Pg.DataverseSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">User registration details</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!System.Net.Mail.MailAddress.TryCreate(request.Email, out _))
            return BadRequest("Invalid email format");

        var (salt, hash) = _userService.CreatePasswordHash(request.Password);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedOn = DateTime.UtcNow
        };

        var result = _userService.CreateUser(user);

        if (!result.Success)
            return Conflict(result.ErrorMessage);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "User created",
            Token = null,
            RefreshToken = null
        });
    }

    /// <summary>
    /// Login with credentials
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with JWT and refresh tokens</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // TODO: Find user by username
        // TODO: Verify password against stored hash
        // TODO: Generate JWT token
        // TODO: Generate refresh token (if using refresh token flow)

        throw new NotImplementedException("User login is not yet implemented");
    }

    /// <summary>
    /// Logout - invalidate refresh token
    /// </summary>
    /// <returns>Logout confirmation</returns>
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // TODO: Invalidate refresh token
        // TODO: Optional: Add token to blacklist if using blacklist strategy

        throw new NotImplementedException("Logout is not yet implemented");
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest("Refresh token is required");

        // TODO: Validate refresh token
        // TODO: Check if refresh token is not expired or blacklisted
        // TODO: Generate new JWT token

        throw new NotImplementedException("Token refresh is not yet implemented");
    }

    /// <summary>
    /// Get current authenticated user
    /// </summary>
    /// <returns>Current user information</returns>
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        // TODO: Extract user ID from JWT token claims
        // TODO: Fetch user details from database
        // TODO: Return user DTO without sensitive information

        throw new NotImplementedException("Get current user is not yet implemented");
    }
}