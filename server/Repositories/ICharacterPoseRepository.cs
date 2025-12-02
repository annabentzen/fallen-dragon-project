using DragonGame.Models;

namespace DragonGame.Repositories;

public interface ICharacterPoseRepository : IRepository<CharacterPose>
{
    Task<List<CharacterPose>> GetAllPosesAsync();
}