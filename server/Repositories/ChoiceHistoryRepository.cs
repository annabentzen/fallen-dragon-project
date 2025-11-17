using DragonGame.Data;
using Microsoft.EntityFrameworkCore;
using DragonGame.Models;

namespace DragonGame.Repositories
{
    public class ChoiceHistoryRepository : Repository<ChoiceHistory>, IChoiceHistoryRepository
    {
        public ChoiceHistoryRepository(AppDbContext context) : base(context)
        {
        }

        // Get all ChoiceHistory entries for a given session
        public async Task<IEnumerable<ChoiceHistory>> GetBySessionIdAsync(int sessionId)
        {
            return await _dbSet
                .Where(c => c.PlayerSessionId == sessionId)
                .ToListAsync();
        }

        // Alias / alternative method for clarity
        public async Task<IEnumerable<ChoiceHistory>> GetChoicesForSessionAsync(int sessionId)
        {
            return await GetBySessionIdAsync(sessionId);
        }
    }
}

