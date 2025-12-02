using DragonGame.Models;

namespace DragonGame.Services;

public interface IPoseService
{
    Task<IEnumerable<CharacterPose>> GetAllPosesAsync();
    Task<CharacterPose?> GetPoseByIdAsync(int id);
    Task AddPoseAsync(CharacterPose pose);
    Task UpdatePoseAsync(CharacterPose pose);
    Task DeletePoseAsync(int id);
}