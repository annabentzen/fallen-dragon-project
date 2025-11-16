using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;


namespace DragonGame.Repositories
{
    public class CharacterRepository : Repository<Character>, ICharacterRepository
    {
        private readonly new AppDbContext _context;

        public CharacterRepository(AppDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<IEnumerable<Character>> GetAllAsync()
        {
            return await _context.Characters
                .Include(c => c.Pose)
                .ToListAsync();
        }

        public override async Task<Character?> GetByIdAsync(int id)
        {
            return await _context.Characters
                .Include(c => c.Pose)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task AddAsync(Character entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _context.Characters.AddAsync(entity);
            await SaveChangesAsync();
        }

        public override async Task UpdateAsync(Character entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Characters.Update(entity);
            await SaveChangesAsync();
        }

        public override async Task DeleteAsync(int id)
        {
            var entity = await _context.Characters.FindAsync(id);
            if (entity != null)
            {
                _context.Characters.Remove(entity);
                await SaveChangesAsync();
            }
        }

        public override async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<Character?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
