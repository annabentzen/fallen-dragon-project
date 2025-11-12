using DragonGame.Models;
using DragonGame.Repositories;


namespace DragonGame.Services
{
    public class PoseService : IPoseService
{
    private readonly IRepository<CharacterPose> _repository;

    public PoseService(IRepository<CharacterPose> repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<CharacterPose>> GetAllPosesAsync() => _repository.GetAllAsync();
    public Task<CharacterPose?> GetPoseByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task AddPoseAsync(CharacterPose pose) => _repository.AddAsync(pose);
    public Task UpdatePoseAsync(CharacterPose pose) => _repository.UpdateAsync(pose);
    public Task DeletePoseAsync(int id) => _repository.DeleteAsync(id);

        public Task<IEnumerable<CharacterPose>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CharacterPose?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CharacterPose> CreateAsync(CharacterPose pose)
        {
            throw new NotImplementedException();
        }

        public Task<CharacterPose?> UpdateAsync(int id, CharacterPose updatedPose)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }

}