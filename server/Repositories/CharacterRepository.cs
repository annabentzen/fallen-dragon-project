using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonGame.Repositories
{
    public class CharacterRepository : Repository<Character>, ICharacterRepository
    {
        public CharacterRepository(AppDbContext context) : base(context)
        {
        }

        // Override GetAllAsync to include Pose
        public override async Task<IEnumerable<Character>> GetAllAsync()
        {
            return await _context.Characters
                .Include(c => c.Pose)
                .ToListAsync();
        }

        // Override GetByIdAsync to include Pose
        public override async Task<Character?> GetByIdAsync(int id)
        {
            return await _context.Characters
                .Include(c => c.Pose)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<Character?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
