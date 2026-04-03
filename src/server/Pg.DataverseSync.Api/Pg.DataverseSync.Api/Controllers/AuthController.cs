using Azure.Core;
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
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
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

        var result = await _userService.CreateUser(user);

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
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Find user by username
        var user = await _userService.GetUserDetailsByUsernameAsync(request.Username);
        if (user is null)
            return Unauthorized(new { message = "Invalid credentials" });

        // Verify password against stored hash
        if (!user.VerifyPassword(request.Password))
            return Unauthorized(new { message = "Invalid credentials" });

        // Generate JWT token
        var token = _tokenService.GenerateJwtToken(user.Id, user.Username, user.Email);

        // Generate refresh token (if using refresh token flow)
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Store refresh token in DB
        await _tokenService.StoreRefreshTokenAsync(user.Id, refreshToken, expirationDays: 30);

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
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest("Refresh token is required");

        // Invalidate refresh token in DB
        await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);


        return Ok(new { message = "Logout successful" });
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        // Validate refresh token from DB
        var storedToken = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken);

        if (storedToken is null || storedToken.RevokedAt is not null)
            return Unauthorized(new { message = "Invalid or revoked refresh token" });

        if (storedToken.ExpiresAt < DateTime.UtcNow)
            return Unauthorized(new { message = "Refresh token expired" });

        var user = await _userService.GetUserDetailsByIdAsync(storedToken.UserId);
        if (user is null)
            return Unauthorized();

        // Generate new tokens
        var newJwt = _tokenService.GenerateJwtToken(user.Id, user.Username, user.Email);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Revoke old refresh token and store new one
        await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
        await _tokenService.StoreRefreshTokenAsync(user.Id, newRefreshToken);

        return Ok(new AuthResponse
        {
            Success = true,
            Token = newJwt,
            RefreshToken = newRefreshToken
        });
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