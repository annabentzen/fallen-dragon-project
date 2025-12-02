using DragonGame.Models;

namespace DragonGame.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}