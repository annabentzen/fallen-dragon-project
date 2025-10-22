using System.Collections.Generic;
using System.Threading.Tasks;
using DragonGame.Models;

namespace DragonGame.Repositories
{
    public interface ICharacterRepository
    {
        Task<List<Character>> GetAllAsync();
        Task<Character?> GetByIdAsync(int id);
        Task AddAsync(Character character);
        Task UpdateAsync(Character character);
        Task DeleteAsync(int id);
    }
}