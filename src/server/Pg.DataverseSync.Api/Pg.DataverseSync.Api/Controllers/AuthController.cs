using Pg.DataverseSync.Api.Application.Services.Interfaces;
using Pg.DataverseSync.Api.Domain;
using Pg.DataverseSync.Api.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pg.DataverseSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
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

        var user = new User
        {
            Username = request.Username,
            Email = request.Email, 
            CreatedOn = DateTime.UtcNow
        };

        user.GeneratePasswordHash(request.Password);

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

        // Find user by username
        var user = _userService.GetUserDetailsByUsername(request.Username);
        if (user is null)
            return Unauthorized(new { message = "Invalid credentials" });

        // Verify password against stored hash
        if (!user.VerifyPassword(request.Password))
            return Unauthorized(new { message = "Invalid credentials" });

        // Generate JWT token
        var token = _tokenService.GenerateJwtToken(user.Id, user.Username, user.Email);

        // Generate refresh token (if using refresh token flow)
        var refreshToken = _tokenService.GenerateRefreshToken();

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            RefreshToken = refreshToken
        });
    }

    /// <summary>
    /// Logout - invalidate refresh token
    /// </summary>
    /// <returns>Logout confirmation</returns>
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // TODO: Invalidate refresh token in database
        // TODO: Optional: Add token to blacklist if using blacklist strategy

        return Ok(new { message = "Logout successful" });
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