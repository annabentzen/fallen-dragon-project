using DragonGame.Models;

namespace DragonGame.Repositories
{
    public interface ICharacterPoseRepository : IRepository<CharacterPose>
    {
        Task<CharacterPose?> GetByNameAsync(string name);
    }
}
