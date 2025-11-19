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
        private readonly AppDbContext _context;

        public PlayerSessionService(
            IPlayerSessionRepository sessionRepo,
            ICharacterRepository characterRepo,
            AppDbContext context)
        {
            _sessionRepo = sessionRepo ?? throw new ArgumentNullException(nameof(sessionRepo));
            _characterRepo = characterRepo ?? throw new ArgumentNullException(nameof(characterRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
                Hair = session.Character.Hair,
                Face = session.Character.Face,
                Outfit = session.Character.Outfit,
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
            character.Hair = updatedCharacter.Hair;
            character.Face = updatedCharacter.Face;
            character.Outfit = updatedCharacter.Outfit;
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

        // ---------- GET CURRENT ACT WITH CHOICES ----------
        public async Task<Act?> GetCurrentActAsync(int sessionId)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId);

            if (session == null)
                return null;

            // Fresh session → force start at Act 1
            int actNumber = session.CurrentActNumber == 0 ? 1 : session.CurrentActNumber;

            var act = await _context.Acts
                .Include(a => a.Choices)
                // ← VERY IMPORTANT
                .FirstOrDefaultAsync(a => 
                    a.StoryId == session.StoryId && 
                    a.ActNumber == actNumber);

            // If still null (should never happen if seed is correct), fallback to real Act 1
            if (act == null && actNumber != 1)
            {
                act = await _context.Acts
                    .Include(a => a.Choices)
                    .FirstOrDefaultAsync(a => a.StoryId == session.StoryId && a.ActNumber == 1);

                if (act != null)
                {
                    session.CurrentActNumber = 1;
                    await _context.SaveChangesAsync();
                }
            }

            return act;
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
