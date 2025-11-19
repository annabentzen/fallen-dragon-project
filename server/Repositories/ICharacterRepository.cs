using DragonGame.Models;

namespace DragonGame.Repositories
{
    public interface ICharacterRepository : IRepository<Character>
    {
        Task<Character?> GetByNameAsync(string name);
    }
}