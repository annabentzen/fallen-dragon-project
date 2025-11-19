using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonGame.Repositories
{
    public class PlayerSessionRepository : Repository<PlayerSession>, IPlayerSessionRepository
    {
        private readonly new AppDbContext _context;

        public PlayerSessionRepository(AppDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<PlayerSession?> GetByIdAsync(int id)
            => await _context.PlayerSessions
                .Include(s => s.Character)
                .FirstOrDefaultAsync(s => s.SessionId == id);

        public override async Task<IEnumerable<PlayerSession>> GetAllAsync()
            => await _context.PlayerSessions
                .Include(s => s.Character)
                .ToListAsync();

        public override async Task AddAsync(PlayerSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            await _context.PlayerSessions.AddAsync(session);
            await SaveChangesAsync();
        }

        public override async Task UpdateAsync(PlayerSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            _context.PlayerSessions.Update(session);
            await SaveChangesAsync();
        }

        public override async Task DeleteAsync(int id)
        {
            var entity = await _context.PlayerSessions.FindAsync(id);
            if (entity != null)
            {
                _context.PlayerSessions.Remove(entity);
                await SaveChangesAsync();
            }
        }

        public override async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<PlayerSession> Query()
            => _context.PlayerSessions.AsQueryable();

        public async Task<PlayerSession?> GetSessionWithCharacterAsync(int sessionId)
        {
            return await _context.PlayerSessions
                .Include(s => s.Character)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }


        public async Task<PlayerSession?> GetSessionByIdWithChoicesAsync(int sessionId)
        {
            return await _context.PlayerSessions
                .Include(s => s.CurrentAct)
                    .ThenInclude(a => a!.Choices)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }
    }
}

