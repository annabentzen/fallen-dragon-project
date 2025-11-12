using YourNamespace.Models;

namespace DragonGame.Repositories
{
    public interface IChoiceHistoryRepository : IRepository<ChoiceHistory>
    {
        Task<IEnumerable<ChoiceHistory>> GetBySessionIdAsync(int sessionId);
        Task<IEnumerable<ChoiceHistory>> GetChoicesForSessionAsync(int sessionId);
    }
}
