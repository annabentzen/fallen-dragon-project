using DragonGame.Models;

namespace DragonGame.Services
{
    public interface IPlayerSessionService
    {
        Task<PlayerSession?> GetByIdAsync(int id);
        Task AddAsync(PlayerSession session);
        Task SaveChangesAsync();  // add this line
    }
}
