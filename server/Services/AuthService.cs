using DragonGame.Data;
using DragonGame.Dtos.Auth;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context, 
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
        {
            _logger.LogWarning("Registration failed - username exists: {Username}", dto.Username);
            return null;
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User registered: {Username} (ID: {UserId})", user.Username, user.UserId);

        return new AuthResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Token = _jwtService.GenerateToken(user)
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed for: {Username}", dto.Username);
            return null;
        }

        _logger.LogInformation("User logged in: {Username}", user.Username);

        return new AuthResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Token = _jwtService.GenerateToken(user)
        };
    }
}