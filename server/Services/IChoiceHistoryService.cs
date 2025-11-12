using DragonGame.Models;

namespace DragonGame.Services
{
    public interface IChoiceHistoryService
    {
        Task AddChoiceAsync(ChoiceHistory choice);
        Task<IEnumerable<ChoiceHistory>> GetChoicesBySessionIdAsync(int sessionId);
    }
}
