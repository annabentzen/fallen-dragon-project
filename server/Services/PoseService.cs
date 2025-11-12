using DragonGame.Models;
using DragonGame.Repositories;


namespace DragonGame.Services
{
    public class PoseService : Repository<CharacterPose>, IPoseService
    {
        private readonly ICharacterPoseRepository _poseRepo;

        public PoseService(ICharacterPoseRepository poseRepo)
        {
            _poseRepo = poseRepo;
        }

        public async Task<IEnumerable<CharacterPose>> GetAllPosesAsync() => await _poseRepo.GetAllPosesAsync();
        public async Task<CharacterPose?> GetPoseByIdAsync(int id) => await _poseRepo.GetByIdAsync(id);
        public async Task AddPoseAsync(CharacterPose pose) => await _poseRepo.AddAsync(pose);
        public async Task UpdatePoseAsync(CharacterPose pose) => await _poseRepo.UpdateAsync(pose);
        public async Task DeletePoseAsync(int id) => await _poseRepo.DeleteAsync(id);

        Task<IEnumerable<CharacterPose>> IPoseService.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<CharacterPose?> IPoseService.GetByIdAsync(int id)
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