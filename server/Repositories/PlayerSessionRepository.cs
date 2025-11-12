using DragonGame.Data;
using DragonGame.Models;
using DragonGame.Repositories;
using Microsoft.EntityFrameworkCore;

public class PlayerSessionRepository : Repository<PlayerSession>, IPlayerSessionRepository
{
    private readonly AppDbContext _context;

    public PlayerSessionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PlayerSession?> GetByIdAsync(int id)
        => await _context.PlayerSessions.FindAsync(id);

    public async Task<IEnumerable<PlayerSession>> GetAllAsync()
        => await _context.PlayerSessions.ToListAsync();

    public async Task AddAsync(PlayerSession session)
        => await _context.PlayerSessions.AddAsync(session);

    public async Task UpdateAsync(PlayerSession session)
        => _context.PlayerSessions.Update(session);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.PlayerSessions.FindAsync(id);
        if (entity != null)
            _context.PlayerSessions.Remove(entity);
    }

    public async Task SaveAsync()
        => await _context.SaveChangesAsync();

    public Task<PlayerSession?> GetSessionWithCharacterAsync(int sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<PlayerSession?> GetSessionByIdWithChoicesAsync(int sessionId)
    {
        throw new NotImplementedException();
    }

    public void Update(PlayerSession entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(PlayerSession entity)
    {
        throw new NotImplementedException();
    }
}
