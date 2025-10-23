using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonGame.Repositories
{
    public class StoryRepository : IStoryRepository
    {
        private readonly AppDbContext _context;
        public StoryRepository(AppDbContext context) => _context = context;

        public async Task<List<Story>> GetAllAsync() => await _context.Stories.Include(s => s.Acts).ThenInclude(a => a.Choices).ToListAsync();
        public async Task<Story> GetByIdAsync(int id) => await _context.Stories.Include(s => s.Acts).ThenInclude(a => a.Choices).FirstOrDefaultAsync(s => s.Id == id);
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
    }
} 