using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Repositories;

public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Character>> GetAllAsync()
    {
        return await _context.Characters
            .Include(c => c.Pose)
            .ToListAsync();
    }

    public override async Task<Character?> GetByIdAsync(int id)
    {
        return await _context.Characters
            .Include(c => c.Pose)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task AddAsync(Character entity)
    {
        await _context.Characters.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public override async Task UpdateAsync(Character entity)
    {
        _context.Characters.Update(entity);
        await _context.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        var entity = await _context.Characters.FindAsync(id);
        if (entity != null)
        {
            _context.Characters.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}