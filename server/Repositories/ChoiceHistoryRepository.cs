using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Repositories;

public class ChoiceHistoryRepository : Repository<ChoiceHistory>, IChoiceHistoryRepository
{
    public ChoiceHistoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ChoiceHistory>> GetBySessionIdAsync(int sessionId)
    {
        return await _dbSet
            .Where(c => c.PlayerSessionId == sessionId)
            .Include(c => c.Choice)
            .OrderBy(c => c.MadeAt)
            .ToListAsync();
    }
}