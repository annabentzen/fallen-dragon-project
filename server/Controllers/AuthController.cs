using DragonGame.Dtos.Auth;
using DragonGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragonGame.Controllers
{
    /// <summary>
    /// Controller for user authentication (register and login)
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
        /// Register a new user
        /// POST: api/auth/register
        /// </summary>
        /// <param name="registerDto">Username and password</param>
        /// <returns>User info with JWT token</returns>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (result == null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Login existing user
        /// POST: api/auth/login
        /// </summary>
        /// <param name="loginDto">Username and password</param>
        /// <returns>User info with JWT token</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(result);
        }
    }
}