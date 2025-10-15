using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonGame.Repositories
{
    public class CharacterPoseRepository : ICharacterPoseRepository
    {
        private readonly DragonGameDbContext _context;

        public CharacterPoseRepository(DragonGameDbContext context)
        {
            _context = context;
        }

        // get all poses
        public async Task<List<CharacterPose>> GetAllAsync()
        {
            return await _context.CharacterPoses.ToListAsync();
        }

        public async Task<CharacterPose?> GetByIdAsync(int id)
        {
            return await _context.CharacterPoses.FindAsync(id);
        }

        // add a new pose
        public async Task AddAsync(CharacterPose pose)
        {
            await _context.CharacterPoses.AddAsync(pose);
            await _context.SaveChangesAsync();
        }

        // update an existing pose
        public async Task UpdateAsync(CharacterPose pose)
        {
            _context.CharacterPoses.Update(pose);
            await _context.SaveChangesAsync();
        }

        // delete a pose by id
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