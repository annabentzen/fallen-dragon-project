using DragonGame.Models;
using DragonGame.Repositories;

namespace DragonGame.Services;

public class PoseService : IPoseService
{
    private readonly ICharacterPoseRepository _repository;

    public PoseService(ICharacterPoseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CharacterPose>> GetAllPosesAsync()
    {
        return await _repository.GetAllPosesAsync();
    }

    public async Task<CharacterPose?> GetPoseByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddPoseAsync(CharacterPose pose)
    {
        await _repository.AddAsync(pose);
    }

    public async Task UpdatePoseAsync(CharacterPose pose)
    {
        await _repository.UpdateAsync(pose);
    }

    public async Task DeletePoseAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}