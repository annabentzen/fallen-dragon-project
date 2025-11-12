using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Repositories;

namespace DragonGame.Services
{
    public class PlayerSessionService : IPlayerSessionService
    {
        private readonly IPlayerSessionRepository _sessionRepo;
        private readonly ICharacterRepository _characterRepo;

        public PlayerSessionService(IPlayerSessionRepository sessionRepo)
        {
            _sessionRepo = sessionRepo;
        }

        public async Task<PlayerSession?> GetSessionAsync(int sessionId)
        {
            return await _sessionRepo.GetByIdAsync(sessionId);
        }

        public async Task<PlayerSession> CreateSessionAsync(CreateSessionDto dto)
        {
            // 1. Create character
            var character = new Character
            {
                Hair = dto.Character.Hair,
                Face = dto.Character.Face,
                Outfit = dto.Character.Outfit,
                PoseId = dto.Character.PoseId
            };
            await _characterRepo.AddAsync(character);
            await _characterRepo.SaveChangesAsync();

            // 2. Create session linked to character
            var session = new PlayerSession
            {
                CharacterId = character.Id,
                Character = character,
                StoryId = dto.StoryId,
                CurrentActNumber = 1,
                IsCompleted = false
            };

            await _sessionRepo.AddAsync(session);
            await _sessionRepo.SaveChangesAsync();

            return session;
        }


        public Task<PlayerSession?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(PlayerSession session)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
