using DragonGame.Models;

namespace DragonGame.Repositories
{
    public interface IStoryRepository : IRepository<Story>
    {
        // Act-specific methods
        Task<Act?> GetActByIdAsync(int actId);
        Task<Act?> GetActWithChoicesAsync(int storyId, int actNumber);
        Task<IEnumerable<Act>> GetAllActsAsync(int storyId);
    }
}