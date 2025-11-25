using DragonGame.Dtos;
using DragonGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragonGame.Controllers
{
    /// <summary>
    /// Handles user authentication endpoints (register, login)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// POST /api/auth/register
        /// Creates a new user account
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);

                if (result == null)
                {
                    return BadRequest(new { message = "Username or email already exists" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController][Register] ERROR: {ex.Message}");
                return StatusCode(500, new { message = "Registration failed" });
            }
        }

        /// <summary>
        /// POST /api/auth/login
        /// Authenticates user and returns JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);

                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController][Login] ERROR: {ex.Message}");
                return StatusCode(500, new { message = "Login failed" });
            }
        }
    }
}