using DragonGame.Models;

namespace server.Services.Interfaces
{
    public interface ICharacterService
    {
        Task<IEnumerable<Character>> GetAllAsync();
        Task<Character?> GetByIdAsync(int id);
        Task<Character> CreateAsync(Character character);
        Task<Character?> UpdateAsync(int id, Character updatedCharacter);
        Task<bool> DeleteAsync(int id);
    }
}
