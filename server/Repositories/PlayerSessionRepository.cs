using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Repositories;

public class PlayerSessionRepository : Repository<PlayerSession>, IPlayerSessionRepository
{
    public PlayerSessionRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<PlayerSession?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Character)
            .FirstOrDefaultAsync(s => s.SessionId == id);
    }

    public override async Task<IEnumerable<PlayerSession>> GetAllAsync()
    {
        return await _dbSet
            .Include(s => s.Character)
            .ToListAsync();
    }

    public async Task<PlayerSession?> GetWithCharacterAsync(int sessionId)
    {
        return await _dbSet
            .Include(s => s.Character)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);
    }

    public async Task<PlayerSession?> GetWithChoicesAsync(int sessionId)
    {
        return await _dbSet
            .Include(s => s.Character)
            .Include(s => s.CurrentAct)
                .ThenInclude(a => a!.Choices)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);
    }

    public IQueryable<PlayerSession> Query()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<bool> DeleteAsync(int sessionId, int userId)
{
    var session = await _dbSet
        .Include(s => s.Character)
        .Include(s => s.Choices)
        .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.UserId == userId);

    if (session == null) return false;

    if (session.Choices?.Any() == true)
    {
        _context.ChoiceHistories.RemoveRange(session.Choices);
    }

    if (session.Character != null)
    {
        _context.Characters.Remove(session.Character);
    }

    _dbSet.Remove(session);
    await _context.SaveChangesAsync();
    return true;
}

}