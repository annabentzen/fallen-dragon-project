
using DragonGame.Repositories;
using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Services
{
    public class ChoiceHistoryService : IChoiceHistoryService
    {
        private readonly AppDbContext _context;

        public ChoiceHistoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChoiceHistory>> GetChoicesBySessionIdAsync(int sessionId)
        {
            return await _context.ChoiceHistories
                .Include(h => h.Choice)
                .Where(h => h.PlayerSessionId == sessionId)
                .OrderBy(h => h.MadeAt)
                .ToListAsync();
        }

        public async Task AddChoiceAsync(ChoiceHistory choice)
        {
            _context.ChoiceHistories.Add(choice);
            await _context.SaveChangesAsync();
        }
    }
}