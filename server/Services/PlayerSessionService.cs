using DragonGame.Dtos;
using DragonGame.Models;

namespace DragonGame.Services
{
    public class PlayerSessionService : IPlayerSessionService
    {
        private readonly IPlayerSessionRepository _sessionRepo;

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
            var session = new PlayerSession
            {
                CharacterName = dto.CharacterName,
                StoryId = dto.StoryId,
                CharacterDesignJson = System.Text.Json.JsonSerializer.Serialize(dto.CharacterDesign),
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
