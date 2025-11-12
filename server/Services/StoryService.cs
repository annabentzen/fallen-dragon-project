using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Repositories;
using System.Text.Json;

namespace DragonGame.Services
{
    public class StoryService : IStoryService
    {
        private readonly IPlayerSessionRepository _sessionRepo;
        private readonly IStoryRepository _storyRepo;

        public StoryService(IPlayerSessionRepository sessionRepo, IStoryRepository storyRepo)
        {
            _sessionRepo = sessionRepo;
            _storyRepo = storyRepo;
        }

        public async Task<PlayerSession> StartStoryAsync(CreateSessionDto dto)
        {
            var session = new PlayerSession
            {
                CharacterName = dto.CharacterName,
                StoryId = dto.StoryId,
                CharacterDesignJson = JsonSerializer.Serialize(dto.CharacterDesign),
                CurrentActNumber = 1,
                IsCompleted = false
            };

            await _sessionRepo.AddAsync(session);

            return session;
        }

        public async Task<object?> GetCurrentActAsync(int sessionId)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId);
            if (session == null) return null;

            var act = await _storyRepo.GetActWithChoicesAsync(session.StoryId, session.CurrentActNumber);
            if (act == null) return null;

            CharacterDesign parsedDesign;
            try
            {
                parsedDesign = !string.IsNullOrEmpty(session.CharacterDesignJson)
                    ? JsonSerializer.Deserialize<CharacterDesign>(session.CharacterDesignJson) ?? new CharacterDesign()
                    : new CharacterDesign();
            }
            catch
            {
                parsedDesign = new CharacterDesign();
            }

            return new
            {
                session = new
                {
                    session.SessionId,
                    session.CharacterName,
                    CharacterDesign = parsedDesign,
                    session.StoryId,
                    session.CurrentActNumber,
                    session.IsCompleted
                },
                act = new
                {
                    act.ActNumber,
                    act.Text,
                    choices = act.Choices.Select(c => new
                    {
                        c.ChoiceId,
                        c.Text,
                        c.ActId,
                        c.NextActNumber
                    }).ToList()
                }
            };
        }

        public async Task<PlayerSession?> MoveToNextActAsync(int sessionId, int nextActNumber)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId);
            if (session == null) return null;

            if (nextActNumber <= 0)
                session.IsCompleted = true;
            else
                session.CurrentActNumber = nextActNumber;

            await _sessionRepo.AddAsync(session);
            return session;
        }

        public async Task<PlayerSession?> UpdateCharacterAsync(int sessionId, CharacterDesign newDesign)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId);
            if (session == null) return null;
            session.CharacterDesignJson = JsonSerializer.Serialize(newDesign);
            await _sessionRepo.AddAsync(session);
            return session;
        }
    }
}