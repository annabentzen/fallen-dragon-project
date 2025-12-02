using DragonGame.Models;
using DragonGame.Repositories;

namespace DragonGame.Services;

public class ChoiceHistoryService : IChoiceHistoryService
{
    private readonly IChoiceHistoryRepository _repository;

    public ChoiceHistoryService(IChoiceHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ChoiceHistory>> GetChoicesBySessionIdAsync(int sessionId)
    {
        return await _repository.GetBySessionIdAsync(sessionId);
    }

    public async Task AddChoiceAsync(ChoiceHistory choice)
    {
        await _repository.AddAsync(choice);
    }
}