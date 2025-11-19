// Services/StoryService.cs
using DragonGame.Data;
using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Services
{
    public class StoryService : IStoryService
    {
        private readonly IPlayerSessionRepository _sessionRepo;
        private readonly IStoryRepository _storyRepo;
        private readonly ICharacterRepository _characterRepo;
        private readonly AppDbContext _context;
        private readonly IChoiceHistoryService _choiceHistoryService;

        public StoryService(
            IPlayerSessionRepository sessionRepo,
            IStoryRepository storyRepo,
            ICharacterRepository characterRepo,
            AppDbContext context, ChoiceHistoryService choiceHistoryService)
        {
            _sessionRepo = sessionRepo;
            _storyRepo = storyRepo;
            _characterRepo = characterRepo;
            _context = context;
            _choiceHistoryService = choiceHistoryService;
        }

        // Starts a new game session with character creation
        public async Task<PlayerSession> StartStoryAsync(CreateSessionDto dto)
        {
            try
            {
                Console.WriteLine($"[StoryService] Starting new story for hero: '{dto.CharacterName}'");

                // Step 1: Create the Character entity first (so we get an ID)
                var character = new Character
                {
                    Hair = dto.Character.Hair,
                    Face = dto.Character.Face,
                    Outfit = dto.Character.Outfit,
                    PoseId = dto.Character.PoseId
                };

                await _characterRepo.AddAsync(character);
                await _characterRepo.SaveChangesAsync(); // Generates character.Id

                Console.WriteLine($"[StoryService] Character created with ID: {character.Id}");

                // Step 2: Create the PlayerSession linked to the character
                var session = new PlayerSession
                {
                    CharacterName = dto.CharacterName,
                    CharacterId = character.Id,
                    StoryId = 1, // Currently only one story
                    CurrentActNumber = 1,
                    IsCompleted = false
                };

                await _sessionRepo.AddAsync(session);
                await _sessionRepo.SaveChangesAsync();

                Console.WriteLine($"[StoryService] Session created! SessionId = {session.SessionId}");
                return session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryService][ERROR] Failed to start story: {ex.Message}\n{ex.StackTrace}");
                throw; // Re-throw so controller can return 500
            }
        }

        // Gets session by ID
        public async Task<PlayerSession?> GetSessionByIdAsync(int id)
        {
            try
            {
                Console.WriteLine($"[StoryService] Fetching session {id}");
                return await _sessionRepo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryService][ERROR] GetSessionByIdAsync({id}) failed: {ex.Message}");
                throw;
            }
        }

        // Returns current act + choices + session data (with character design)
        public async Task<object?> GetCurrentActAsync(int sessionId)
        {
            try
            {
                Console.WriteLine($"[StoryService] Loading current act for session {sessionId}");

                var session = await _sessionRepo.GetByIdAsync(sessionId);
                if (session == null)
                {
                    Console.WriteLine($"[StoryService] Session {sessionId} not found");
                    return null;
                }

                // Load character navigation property
                await _context.Entry(session).Reference(s => s.Character).LoadAsync();

                var act = await _storyRepo.GetActWithChoicesAsync(session.StoryId, session.CurrentActNumber);
                if (act == null)
                {
                    Console.WriteLine($"[StoryService] No act found for StoryId {session.StoryId}, Act {session.CurrentActNumber}");
                    return null;
                }

                Console.WriteLine($"[StoryService] Returning Act {act.ActNumber} with {act.Choices.Count} choices");
                return new
                {
                    session = new
                    {
                        session.SessionId,
                        session.CharacterName,
                        Character = session.Character != null ? new
                        {
                            session.Character.Hair,
                            session.Character.Face,
                            session.Character.Outfit,
                            session.Character.PoseId
                        } : null,
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
                            c.NextActNumber
                        }).ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryService][ERROR] GetCurrentActAsync({sessionId}) failed: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        // Moves player to next act (or ends game if nextActNumber <= 0)
      public async Task<PlayerSession?> MoveToNextActAsync(int sessionId, int nextActNumber)
{
    try
    {
        Console.WriteLine($"[StoryService] Moving session {sessionId} → Act {nextActNumber}");

        // Load session + current act + choices
        var session = await _sessionRepo.GetSessionByIdWithChoicesAsync(sessionId);
        if (session == null || session.CurrentAct == null)
        {
            Console.WriteLine($"[StoryService] Session or CurrentAct not found: {sessionId}");
            return null;
        }

        // Find the choice that leads to nextActNumber
        var selectedChoice = session.CurrentAct.Choices
            ?.FirstOrDefault(c => c.NextActNumber == nextActNumber);

        // Record choice in history (only if valid choice exists)
        if (selectedChoice != null)
        {
            var historyEntry = new ChoiceHistory
            {
                PlayerSessionId = session.SessionId,
                ActNumber = session.CurrentAct.ActNumber,
                ChoiceId = selectedChoice.ChoiceId,
                MadeAt = DateTime.UtcNow
            };

            await _choiceHistoryService.AddChoiceAsync(historyEntry);
            Console.WriteLine($"[StoryService] Recorded choice: Act {historyEntry.ActNumber} → Choice {selectedChoice.ChoiceId}");
        }
        else
        {
            Console.WriteLine($"[StoryService] WARNING: No choice found for nextActNumber {nextActNumber}. Proceeding anyway.");
            // Optional: throw new InvalidOperationException("Invalid choice");
        }

        // Update session state
        if (nextActNumber <= 0)
        {
            session.IsCompleted = true;
            Console.WriteLine($"[StoryService] Story completed for session {sessionId}");
        }
        else
        {
            session.CurrentActNumber = nextActNumber;
        }

        await _sessionRepo.UpdateAsync(session);
        await _sessionRepo.SaveChangesAsync();

        return session;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[StoryService][ERROR] MoveToNextAct failed: {ex.Message}\n{ex.StackTrace}");
        throw;
    }
}

        // Gets just the Character entity for a session (used in edit mode)
        public async Task<Character?> GetCharacterForSessionAsync(int sessionId)
        {
            try
            {
                Console.WriteLine($"[StoryService] Fetching character for session {sessionId}");
                var session = await _context.PlayerSessions
                    .Include(s => s.Character)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session?.Character == null)
                    Console.WriteLine($"[StoryService] No character found for session {sessionId}");

                return session?.Character;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryService][ERROR] GetCharacterForSessionAsync failed: {ex.Message}");
                throw;
            }
        }

        // Updates character appearance mid-game
        public async Task<PlayerSession?> UpdateCharacterAsync(int sessionId, Character newDesign)
        {
            try
            {
                Console.WriteLine($"[StoryService] Updating character design for session {sessionId}");

                var session = await _sessionRepo.GetByIdAsync(sessionId);
                if (session == null)
                {
                    Console.WriteLine($"[StoryService] Session {sessionId} not found");
                    return null;
                }

                await _context.Entry(session).Reference(s => s.Character).LoadAsync();
                var character = session.Character;

                if (character == null)
                {
                    Console.WriteLine($"[StoryService] Character not found for session {sessionId}");
                    return session;
                }

                // Apply new design
                character.Hair = newDesign.Hair;
                character.Face = newDesign.Face;
                character.Outfit = newDesign.Outfit;
                character.PoseId = newDesign.PoseId;

                await _characterRepo.UpdateAsync(character);
                await _characterRepo.SaveChangesAsync();

                Console.WriteLine($"[StoryService] Character updated successfully!");
                return session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryService][ERROR] UpdateCharacterAsync failed: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

    }
}