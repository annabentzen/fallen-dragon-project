using DragonGame.Data;
using DragonGame.Models;
using DragonGame.Repositories;
using Microsoft.EntityFrameworkCore;

public class PlayerSessionRepository : Repository<PlayerSession>, IPlayerSessionRepository
{

    public PlayerSessionRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<PlayerSession?> GetByIdAsync(int id)
        => await _context.PlayerSessions.FindAsync(id);

    public override async Task<IEnumerable<PlayerSession>> GetAllAsync()
        => await _context.PlayerSessions.ToListAsync();

    public override async Task AddAsync(PlayerSession session)
    {
        Console.WriteLine($"_context is null? {_context == null}");
        await _context.PlayerSessions.AddAsync(session);
    }


    public override async Task UpdateAsync(PlayerSession session)
        => _context.PlayerSessions.Update(session);

    public override async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        var entity = await _context.PlayerSessions.FindAsync(id);
        if (entity != null)
            _context.PlayerSessions.Remove(entity);
    }

    public async Task SaveAsync()
        => await _context.SaveChangesAsync();

    public async Task<PlayerSession?> GetSessionWithCharacterAsync(int sessionId)
    {
        return await _context.PlayerSessions
            .Include(s => s.Character)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);
    }


    public Task<PlayerSession?> GetSessionByIdWithChoicesAsync(int sessionId)
    {
        throw new NotImplementedException();
    }

    public override void Update(PlayerSession entity)
    {
        throw new NotImplementedException();
    }

    public override void Delete(PlayerSession entity)
    {
        throw new NotImplementedException();
    }

    public IQueryable<PlayerSession> Query()
        => _context.PlayerSessions.AsQueryable();
}
