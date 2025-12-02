using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Repositories;

public class StoryRepository : Repository<Story>, IStoryRepository
{
    public StoryRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<Story?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Acts)
                .ThenInclude(a => a.Choices)
            .FirstOrDefaultAsync(s => s.StoryId == id);
    }

    public override async Task<IEnumerable<Story>> GetAllAsync()
    {
        return await _dbSet
            .Include(s => s.Acts)
                .ThenInclude(a => a.Choices)
            .ToListAsync();
    }

    public async Task<Act?> GetActByNumberAsync(int storyId, int actNumber)
    {
        return await _context.Acts
            .FirstOrDefaultAsync(a => a.StoryId == storyId && a.ActNumber == actNumber);
    }

    public async Task<Act?> GetActWithChoicesAsync(int storyId, int actNumber)
    {
        return await _context.Acts
            .Include(a => a.Choices)
            .FirstOrDefaultAsync(a => a.StoryId == storyId && a.ActNumber == actNumber);
    }
}