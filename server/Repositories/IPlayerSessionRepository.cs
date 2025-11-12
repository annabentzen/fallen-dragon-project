using DragonGame.Models;
using DragonGame.Repositories;

public interface IPlayerSessionRepository : IRepository<PlayerSession>
    {
        Task<PlayerSession?> GetSessionWithCharacterAsync(int sessionId);
        Task<PlayerSession?> GetSessionByIdWithChoicesAsync(int sessionId);
    }
