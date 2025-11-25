using DragonGame.Dtos.Auth;

namespace DragonGame.Services
{
    /// <summary>
    /// Service for handling user authentication and registration
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Register a new user
        /// </summary>
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// Login existing user and return JWT token
        /// </summary>
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    }
}