using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonGame.Repositories
{
    /// <summary>    
    /// Repository for managing CharacterPose entities.
    /// Implements CRUD operations for pose options available to players during character creation.
    /// </summary>
    public class CharacterPoseRepository : ICharacterPoseRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of CharacterPoseRepository with db context.
        /// </summary>
        /// <param name="context">Entity framework database context</param>
        public CharacterPoseRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all character poses.
        /// Used to populate pose selection options in character builder
        /// </summary>
        /// <returns>A list of character poses</returns>
        public async Task<List<CharacterPose>> GetAllPosesAsync()
        {
            return await _context.CharacterPoses.ToListAsync();
        }

        /// <summary>
        /// Gets a character pose by its ID.
        /// </summary>
        /// <param name="id">The ID of the character pose</param>
        /// <returns>The pose if found, otherwise null</returns>
        public async Task<CharacterPose?> GetByIdAsync(int id)
        {
            return await _context.CharacterPoses
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        /// <summary>
        /// Adds a new character pose.
        /// </summary>
        /// <param name="pose"> The pose to add</param>
        public async Task AddAsync(CharacterPose pose)
        {
            await _context.CharacterPoses.AddAsync(pose);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing pose in the database.
        /// </summary>
        /// <param name="pose">The pose with updated values</param>
        public async Task UpdateAsync(CharacterPose pose)
        {
            _context.CharacterPoses.Update(pose);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a pose from the database by ID.
        /// Only deletes if the pose exists.
        /// </summary>
        /// <param name="id">The unique identifier of the pose to delete</param>
        public async Task DeleteAsync(int id)
        {
            var pose = await _context.CharacterPoses.FindAsync(id);
            if (pose != null)
            {
                _context.CharacterPoses.Remove(pose);
                await _context.SaveChangesAsync();
            }
        }
    }
}
