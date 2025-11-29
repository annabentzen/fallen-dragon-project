using DragonGame.Dtos;
using DragonGame.Models;

namespace DragonGame.Services
{
    public interface IPlayerSessionService
    {
        Task<PlayerSession> CreateSessionAsync(CreateSessionDto dto, int userId);
        Task<PlayerSession?> GetByIdAsync(int id);
        Task AddAsync(PlayerSession session);
        Task SaveChangesAsync();  
    }
}
