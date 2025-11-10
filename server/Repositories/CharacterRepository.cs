using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonGame.Repositories
{
    /// <summary>
    /// Repository for managing Character entities.
    /// Implements CRUD operations
    /// </summary>
    public class CharacterRepository : ICharacterRepository
    {
        private readonly AppDbContext _context;
        /// <summary>
        /// Initializes a new instance of CharacterRepository with db context.
        /// </summary>
        /// <param name="context">Entity framework database context</param>
        public CharacterRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all characters with their poses.
        /// </summary>
        /// <returns>List of characters with poses</returns>
        public async Task<List<Character>> GetAllAsync()
        {
            return await _context.Characters
                .Include(c => c.Pose) // include pose data
                .ToListAsync();
        }
        /// <summary>
        /// Gets a character by id with pose.
        /// </summary>
        /// <param name="id">Character id</param>
        /// <returns>Character with pose</returns>
        public async Task<Character?> GetByIdAsync(int id)
        {
            return await _context.Characters
                .Include(c => c.Pose) // include pose data
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        /// <summary>
        /// Adds a new character.
        /// </summary>
        /// <param name="character">Character to add</param>
        public async Task AddAsync(Character character)
        {
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Updates an existing character.
        /// </summary>
        /// <param name="character">Character to update</param>
        public async Task UpdateAsync(Character character)
        {
            _context.Characters.Update(character);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a character by id.
        /// </summary>
        /// <param name="id">Character id to delete</param>
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
