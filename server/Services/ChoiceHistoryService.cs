
using DragonGame.Repositories;
using YourNamespace.Models;

namespace DragonGame.Services
{
  public class ChoiceHistoryService : IChoiceHistoryService
    {
        private readonly IChoiceHistoryRepository _repo;

        public ChoiceHistoryService(IChoiceHistoryRepository repo)
        {
            _repo = repo;
        }

        public async Task AddChoiceAsync(ChoiceHistory choice)
        {
            await _repo.AddAsync(choice);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChoiceHistory>> GetChoicesBySessionIdAsync(int sessionId)
        {
            return await _repo.GetBySessionIdAsync(sessionId);
        }
    }
}