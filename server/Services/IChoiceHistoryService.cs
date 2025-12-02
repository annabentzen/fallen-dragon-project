using DragonGame.Models;

namespace DragonGame.Services;

public interface IChoiceHistoryService
{
    Task<IEnumerable<ChoiceHistory>> GetChoicesBySessionIdAsync(int sessionId);
    Task AddChoiceAsync(ChoiceHistory choice);
}