using DragonGame.Models;

namespace DragonGame.Repositories;

public interface IChoiceHistoryRepository : IRepository<ChoiceHistory>
{
    Task<IEnumerable<ChoiceHistory>> GetBySessionIdAsync(int sessionId);
}