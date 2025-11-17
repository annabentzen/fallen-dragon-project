using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Repositories
{
    public class StoryRepository : Repository<Story>, IStoryRepository
    {
        public StoryRepository(AppDbContext context) : base(context)
        {
        }

        //Story methods
        public async Task<List<Story>> GetAllAsync() => await _context.Stories.Include(s => s.Acts).ThenInclude(a => a.Choices).ToListAsync();
        public async Task<Story> GetByIdAsync(int id)
        {
            var story = await _context.Stories
                .Include(s => s.Acts)
                .ThenInclude(a => a.Choices)
                .FirstOrDefaultAsync(s => s.StoryId == id);

            if (story == null)
                throw new KeyNotFoundException($"Story with ID {id} not found.");

            return story;
        }
        public async Task<Story> CreateAsync(Story story)
        {
            _context.Stories.Add(story);
            await _context.SaveChangesAsync();
            return story;
        }
        public async Task UpdateAsync(Story story)
        {
            _context.Stories.Update(story);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var story = await _context.Stories.FindAsync(id);
            if (story != null)
            {
                _context.Stories.Remove(story);
                await _context.SaveChangesAsync();
            }
        }


        //Act methods
        public async Task<IEnumerable<Act>> GetAllActsAsync() =>
            await _context.Acts.ToListAsync();

        public async Task<Act?> GetActByIdAsync(int id) =>
            await _context.Acts.FirstOrDefaultAsync(a => a.ActId == id);

        public async Task<Act?> GetActWithChoicesAsync(int storyId, int actNumber)
        {
            return await _context.Acts
                .Include(a => a.Choices)
                .FirstOrDefaultAsync(a => a.StoryId == storyId && a.ActNumber == actNumber);
        }

        public async Task AddActAsync(Act act)
        {
            _context.Acts.Add(act);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateActAsync(Act act)
        {
            _context.Acts.Update(act);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteActAsync(int id)
        {
            var act = await _context.Acts.FindAsync(id);
            if (act != null)
            {
                _context.Acts.Remove(act);
                await _context.SaveChangesAsync();
            }
        }

        public Task<IEnumerable<Act>> GetAllActsAsync(int storyId)
        {
            throw new NotImplementedException();
        }
    }
} 
