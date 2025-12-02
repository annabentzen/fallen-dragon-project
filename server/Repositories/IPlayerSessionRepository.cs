using DragonGame.Models;

namespace DragonGame.Repositories;

public interface IPlayerSessionRepository : IRepository<PlayerSession>
{
    Task<PlayerSession?> GetWithCharacterAsync(int sessionId);
    Task<PlayerSession?> GetWithChoicesAsync(int sessionId);
    IQueryable<PlayerSession> Query();
}