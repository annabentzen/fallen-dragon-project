using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Repositories
{
    public class CharacterPoseRepository : Repository<CharacterPose>, ICharacterPoseRepository
    {
        private readonly AppDbContext _context;

        public CharacterPoseRepository(AppDbContext context) : base(context)
        {
        }

        // Get all poses
        public async Task<List<CharacterPose>> GetAllPosesAsync()
        {
            return await _context.CharacterPoses.ToListAsync();
        }

      // Get pose by id
        public async Task<CharacterPose?> GetByIdAsync(int id)
        {
            return await _context.CharacterPoses
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        // Add a new pose
        public async Task AddAsync(CharacterPose pose)
        {
            await _context.CharacterPoses.AddAsync(pose);
            await _context.SaveChangesAsync();
        }

        // Update existing pose
        public async Task UpdateAsync(CharacterPose pose)
        {
            _context.CharacterPoses.Update(pose);
            await _context.SaveChangesAsync();
        }

        // Delete pose
        public async Task DeleteAsync(int id)
        {
            var pose = await _context.CharacterPoses.FindAsync(id);
            if (pose != null)
            {
                _context.CharacterPoses.Remove(pose);
                await _context.SaveChangesAsync();
            }
        }

        public Task<IEnumerable<CharacterPose>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(CharacterPose entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(CharacterPose entity)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CharacterPose?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
