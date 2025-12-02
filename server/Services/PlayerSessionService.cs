using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Services;

public class PlayerSessionService : IPlayerSessionService
{
    private readonly IPlayerSessionRepository _sessionRepo;
    private readonly ICharacterRepository _characterRepo;
    private readonly IChoiceHistoryService _choiceHistoryService;
    private readonly ILogger<PlayerSessionService> _logger;

    public PlayerSessionService(
        IPlayerSessionRepository sessionRepo,
        ICharacterRepository characterRepo,
        IChoiceHistoryService choiceHistoryService,
        ILogger<PlayerSessionService> logger)
    {
        _sessionRepo = sessionRepo;
        _characterRepo = characterRepo;
        _choiceHistoryService = choiceHistoryService;
        _logger = logger;
    }

    public async Task<PlayerSession> CreateSessionAsync(CreateSessionDto dto, int userId)
    {
        var character = new Character
        {
            Head = dto.Character.Head,
            Body = dto.Character.Body,
            PoseId = dto.Character.PoseId
        };
        await _characterRepo.AddAsync(character);

        var session = new PlayerSession
        {
            UserId = userId,
            CharacterId = character.Id,
            Character = character,
            CharacterName = dto.CharacterName,
            StoryId = dto.StoryId,
            CurrentActNumber = 1,
            IsCompleted = false
        };
        await _sessionRepo.AddAsync(session);

        _logger.LogInformation(
            "Session {SessionId} created for user {UserId}",
            session.SessionId,
            userId);

        return session;
    }

    public async Task<PlayerSession?> GetSessionAsync(int sessionId)
    {
        return await _sessionRepo.GetWithCharacterAsync(sessionId);
    }

    public async Task<PlayerSessionDto?> GetSessionDtoAsync(int sessionId)
    {
        var session = await _sessionRepo.GetWithCharacterAsync(sessionId);
        if (session?.Character == null) return null;

        return new PlayerSessionDto
        {
            SessionId = session.SessionId,
            UserId = session.UserId,
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

    public async Task<Character?> GetCharacterForSessionAsync(int sessionId)
    {
        var session = await _sessionRepo.GetWithCharacterAsync(sessionId);
        return session?.Character;
    }

    public async Task<PlayerSession?> UpdateCharacterAsync(int sessionId, Character updatedCharacter)
    {
        var session = await _sessionRepo.GetWithCharacterAsync(sessionId);
        if (session?.Character == null)
        {
            throw new KeyNotFoundException($"Session {sessionId} or character not found");
        }

        session.Character.Head = updatedCharacter.Head;
        session.Character.Body = updatedCharacter.Body;
        session.Character.PoseId = updatedCharacter.PoseId;

        await _characterRepo.UpdateAsync(session.Character);

        _logger.LogInformation("Character updated for session {SessionId}", sessionId);

        return session;
    }

    public async Task<PlayerSession?> MoveToNextActAsync(int sessionId, int nextActNumber)
    {
        var session = await _sessionRepo.GetWithChoicesAsync(sessionId);
        if (session?.CurrentAct == null)
        {
            _logger.LogWarning("Session or CurrentAct not found: {SessionId}", sessionId);
            return null;
        }

        var selectedChoice = session.CurrentAct.Choices
            ?.FirstOrDefault(c => c.NextActNumber == nextActNumber);

        if (selectedChoice == null) throw new InvalidOperationException("Next Act does not match choice");
        var historyEntry = new ChoiceHistory
        {
            PlayerSessionId = session.SessionId,
            ActNumber = session.CurrentAct.ActNumber,
            ChoiceId = selectedChoice.ChoiceId,
            MadeAt = DateTime.UtcNow
        };

        await _choiceHistoryService.AddChoiceAsync(historyEntry);

        _logger.LogInformation(
            "Choice recorded: Session {SessionId}, Act {ActNumber} → Choice {ChoiceId}",
            sessionId, historyEntry.ActNumber, selectedChoice.ChoiceId);


        if (nextActNumber <= 0)
        {
            session.IsCompleted = true;
            _logger.LogInformation("Session completed: {SessionId}", sessionId);
        }
        else
        {
            session.CurrentActNumber = nextActNumber;
        }

        await _sessionRepo.UpdateAsync(session);

        return session;
    }
}