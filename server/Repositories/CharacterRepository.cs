using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonGame.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly AppDbContext _context;

        public CharacterRepository(AppDbContext context)
        {
            _context = context;
        }

        // get all characters with their poses
        public async Task<List<Character>> GetAllAsync()
        {
            return await _context.Characters
                .Include(c => c.Pose) // include pose data
                .ToListAsync();
        }
        // get character by id with pose
        public async Task<Character?> GetByIdAsync(int id)
        {
            return await _context.Characters
                .Include(c => c.Pose) // include pose data
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        // add a new character
        public async Task AddAsync(Character character)
        {
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();
        }
        // update an existing character
        public async Task UpdateAsync(Character character)
        {
            _context.Characters.Update(character);
            await _context.SaveChangesAsync();
        }

        // delete a character by id
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
