public interface IStoryRepository
{
    Task<List<Story>> GetAllAsync();
    Task<Story> GetByIdAsync(int id);
    Task<Story> CreateAsync(Story story);
    Task UpdateAsync(Story story);
    Task DeleteAsync(int id);
}
