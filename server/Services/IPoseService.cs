using DragonGame.Models;

namespace DragonGame.Services
{
    public interface IPoseService
    {
        Task<IEnumerable<CharacterPose>> GetAllAsync();
        Task<CharacterPose?> GetByIdAsync(int id);
        Task<CharacterPose> CreateAsync(CharacterPose pose);
        Task<CharacterPose?> UpdateAsync(int id, CharacterPose updatedPose);
        Task<bool> DeleteAsync(int id);
    }
}    



