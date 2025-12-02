using DragonGame.Models;

namespace DragonGame.Services;

public interface ICharacterService
{
    Task<IEnumerable<Character>> GetAllAsync();
    Task<Character?> GetByIdAsync(int id);
    Task<Character> CreateAsync(Character character);
    Task<Character?> UpdateAsync(int id, Character character);
    Task<bool> DeleteAsync(int id);
}