using DragonGame.Data;
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
        private readonly ICharacterRepository _characterRepo;
        private readonly AppDbContext _context;

        public StoryService(
            IPlayerSessionRepository sessionRepo,
            IStoryRepository storyRepo,
            ICharacterRepository characterRepo,
            AppDbContext context)
        {
            _sessionRepo = sessionRepo;
            _storyRepo = storyRepo;
            _characterRepo = characterRepo;
            _context = context;
        }

        public async Task<PlayerSession> StartStoryAsync(CreateSessionDto dto)
        {
            // Create Character
            var character = new Character
            {
                Hair = dto.Character.Hair,
                Face = dto.Character.Face,
                Outfit = dto.Character.Outfit,
                PoseId = dto.Character.PoseId
            };

            await _characterRepo.AddAsync(character);
            await _characterRepo.SaveChangesAsync(); // Save to get the generated Id

            // Create PlayerSession
            var session = new PlayerSession
            {
                CharacterName = dto.CharacterName,
                CharacterId = character.Id,  // Link to the new character
                StoryId = dto.StoryId,
                CurrentActNumber = 1,
                IsCompleted = false
            };

            await _sessionRepo.AddAsync(session);
            await _sessionRepo.SaveChangesAsync();

            return session;
        }


        public async Task<object?> GetCurrentActAsync(int sessionId)
    {
        var session = await _sessionRepo.GetByIdAsync(sessionId);
        if (session == null) return null;

        // Load related Character entity
        await _context.Entry(session).Reference(s => s.Character).LoadAsync();

        var act = await _storyRepo.GetActWithChoicesAsync(session.StoryId, session.CurrentActNumber);
        if (act == null) return null;

        var character = session.Character;

        return new
        {
            session = new
            {
                session.SessionId,
                session.CharacterName,
                Character = new
                {
                    character.Hair,
                    character.Face,
                    character.Outfit,
                    character.PoseId
                },
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

        await _sessionRepo.UpdateAsync(session);
        await _sessionRepo.SaveChangesAsync();
        return session;
    }


        public async Task<PlayerSession?> UpdateCharacterAsync(int sessionId, Character newDesign)
    {
        var session = await _sessionRepo.GetByIdAsync(sessionId);
        if (session == null) return null;

        await _context.Entry(session).Reference(s => s.Character).LoadAsync();
        var character = session.Character;

        if (character != null)
        {
            character.Hair = newDesign.Hair;
            character.Face = newDesign.Face;
            character.Outfit = newDesign.Outfit;
            character.PoseId = newDesign.PoseId;
            await _characterRepo.UpdateAsync(character);
            await _characterRepo.SaveChangesAsync();
        }

        return session;
    }

    
    }
}