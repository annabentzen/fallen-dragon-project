using DragonGame.Dtos.Auth;
using DragonGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragonGame.Controllers;

/// <summary>
/// Handles user authentication (register, login).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <response code="200">Registration successful</response>
    /// <response code="400">Username already exists</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        _logger.LogInformation("Registration attempt for {Username}", dto.Username);

        var result = await _authService.RegisterAsync(dto);

        if (result == null)
        {
            _logger.LogWarning("Registration failed - username already exists: {Username}", dto.Username);
            return BadRequest(new { message = "Username already exists" });
        }

        _logger.LogInformation("User registered successfully: {Username}", dto.Username);
        return Ok(result);
    }

    /// <summary>
    /// Authenticates user and returns JWT token.
    /// </summary>
    /// <response code="200">Login successful, returns JWT</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        _logger.LogInformation("Login attempt for {Username}", dto.Username);

        var result = await _authService.LoginAsync(dto);

        if (result == null)
        {
            _logger.LogWarning("Login failed - invalid credentials for {Username}", dto.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }

        _logger.LogInformation("User logged in: {Username}", dto.Username);
        return Ok(result);
    }
}