using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonGame.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly DragonGameDbContext _context;

        public CharacterRepository(DragonGameDbContext context)
        {
            _context = context;
        }

        public async Task<List<Character>> GetAllAsync()
        {
            return await _context.Characters
                .Include(c => c.Power)
                .Include(c => c.Pose)
                .ToListAsync();
        }

        public async Task<Character?> GetByIdAsync(int id)
        {
            return await _context.Characters
                .Include(c => c.Power)
                .Include(c => c.Pose)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Character character)
        {
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Character character)
        {
            _context.Characters.Update(character);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character != null)
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
            }
        }
    }
}
