using DragonGame.Models;
using YourNamespace.Models;

namespace DragonGame.Services
{
    public interface IChoiceHistoryService
    {
        Task AddChoiceAsync(ChoiceHistory choice);
        Task<IEnumerable<ChoiceHistory>> GetChoicesBySessionIdAsync(int sessionId);
    }
}
