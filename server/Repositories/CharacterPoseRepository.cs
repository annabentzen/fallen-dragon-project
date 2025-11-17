using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Repositories
{
    public class CharacterPoseRepository : Repository<CharacterPose>, ICharacterPoseRepository
    {
        public CharacterPoseRepository(AppDbContext context) : base(context)
        {
        }

        // Custom method 
        public async Task<List<CharacterPose>> GetAllPosesAsync()
        {
            return await _context.CharacterPoses.ToListAsync();
        }

        // Override base repository method
        public override async Task<CharacterPose?> GetByIdAsync(int id)
        {
            return await _context.CharacterPoses
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task AddAsync(CharacterPose pose)
        {
            await _context.CharacterPoses.AddAsync(pose);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(CharacterPose pose)
        {
            _context.CharacterPoses.Update(pose);
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(int id)
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
