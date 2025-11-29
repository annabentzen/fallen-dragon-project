using DragonGame.Data;
using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Services
{
    public class PlayerSessionService : IPlayerSessionService
    {
        private readonly IPlayerSessionRepository _sessionRepo;
        private readonly ICharacterRepository _characterRepo;

        public PlayerSessionService(
            IPlayerSessionRepository sessionRepo,
            ICharacterRepository characterRepo)
        {
            _sessionRepo = sessionRepo ?? throw new ArgumentNullException(nameof(sessionRepo));
            _characterRepo = characterRepo ?? throw new ArgumentNullException(nameof(characterRepo));
        }


        // ---------- GET SESSION DTO WITH CHARACTER DESIGN ----------
        public async Task<PlayerSessionDto?> GetSessionDtoAsync(int sessionId)
        {
            var session = await _sessionRepo.Query()
                .Include(ps => ps.Character)
                .FirstOrDefaultAsync(ps => ps.SessionId == sessionId);

            if (session == null) return null;

            return new PlayerSessionDto
            {
                SessionId = session.SessionId,
                CharacterName = session.CharacterName,
                CharacterId = session.CharacterId,
                Head = session.Character.Head,
                Body = session.Character.Body,
                PoseId = session.Character.PoseId,
                StoryId = session.StoryId,
                CurrentActNumber = session.CurrentActNumber,
                IsCompleted = session.IsCompleted
            };
        }


        // ---------- UPDATE CHARACTER DESIGN ----------
       public async Task UpdateCharacterAsync(int sessionId, Character updatedCharacter)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId);
            if (session == null) throw new Exception("Session not found");

            var character = await _characterRepo.GetByIdAsync(session.CharacterId);
            if (character == null) throw new Exception("Character not found");

            // Update character fields
            character.Head = updatedCharacter.Head;
            character.Body = updatedCharacter.Body;
            character.PoseId = updatedCharacter.PoseId;

            await _characterRepo.SaveChangesAsync();
        }




        // ---------- GET SESSION ENTITY ----------
        public async Task<PlayerSession?> GetSessionAsync(int sessionId)
        {
            return await _sessionRepo.Query()
                .Include(ps => ps.Character)
                .FirstOrDefaultAsync(ps => ps.SessionId == sessionId);
        }

        // ---------- CREATE SESSION WITH CHARACTER ----------
        public async Task<PlayerSession> CreateSessionAsync(CreateSessionDto dto)
        {
            // 1. Create character
            var character = new Character
            {
                Head = dto.Character.Head,
                Body = dto.Character.Body,
                PoseId = dto.Character.PoseId
            };
            await _characterRepo.AddAsync(character);
            await _characterRepo.SaveChangesAsync();

            // 2. Create session linked to character
            var session = new PlayerSession
            {
                CharacterId = character.Id,
                Character = character,
                CharacterName = dto.CharacterName ?? "Hero",
                StoryId = dto.StoryId,
                CurrentActNumber = 1,
                IsCompleted = false
            };
            await _sessionRepo.AddAsync(session);
            await _sessionRepo.SaveChangesAsync();

            return session;
        }


        // ---------- GET CHARACTER FOR SESSION ----------
        public async Task<Character> GetCharacterForSessionAsync(int sessionId)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId);
            if (session == null)
                throw new Exception($"No session found with id {sessionId}");

            var character = await _characterRepo.GetByIdAsync(session.CharacterId);
            if (character == null)
                throw new Exception($"No character found with id {session.CharacterId}");

            return character;
        }


        // ---------- ADDITIONAL REPOSITORY METHODS ----------
        public async Task<PlayerSession?> GetByIdAsync(int id)
        {
            return await _sessionRepo.GetByIdAsync(id);
        }

        public async Task AddAsync(PlayerSession session)
        {
            await _sessionRepo.AddAsync(session);
        }

        public async Task SaveChangesAsync()
        {
            await _sessionRepo.SaveChangesAsync();
        }
    }
}
