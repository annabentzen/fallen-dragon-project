using DragonGame.Models;

namespace DragonGame.Repositories;

public interface IStoryRepository : IRepository<Story>
{
    Task<Act?> GetActByNumberAsync(int storyId, int actNumber);
    Task<Act?> GetActWithChoicesAsync(int storyId, int actNumber);
}