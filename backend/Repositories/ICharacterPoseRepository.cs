using DragonGame.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonGame.Repositories
{
    public interface ICharacterPoseRepository
    {
        Task<List<CharacterPose>> GetAllAsync();
        Task<CharacterPose?> GetByIdAsync(int id);
        Task AddAsync(CharacterPose pose);
        Task UpdateAsync(CharacterPose pose);
        Task DeleteAsync(int id);
    }
}