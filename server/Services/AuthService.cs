using DragonGame.Data;
using DragonGame.Dtos;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Services
{
    /// <summary>
    /// Service for handling user authentication (register, login)
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Registers a new user account
        /// Returns null if username or email already exists
        /// </summary>
        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                Console.WriteLine($"[AuthService] Username '{dto.Username}' already exists");
                return null;
            }

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                Console.WriteLine($"[AuthService] Email '{dto.Email}' already exists");
                return null;
            }

            // Hash the password using BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create new user
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Console.WriteLine($"[AuthService] User registered: {user.Username} (ID: {user.UserId})");

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Token = token
            };
        }

        /// <summary>
        /// Authenticates a user with username and password
        /// Returns null if credentials are invalid
        /// </summary>
        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            // Find user by username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null)
            {
                Console.WriteLine($"[AuthService] Login failed: User '{dto.Username}' not found");
                return null;
            }

            // Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                Console.WriteLine($"[AuthService] Login failed: Invalid password for '{dto.Username}'");
                return null;
            }

            Console.WriteLine($"[AuthService] User logged in: {user.Username} (ID: {user.UserId})");

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Token = token
            };
        }
    }
}