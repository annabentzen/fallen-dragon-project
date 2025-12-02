using DragonGame.Dtos;
using DragonGame.Models;

namespace DragonGame.Services;

public interface IPlayerSessionService
{
    Task<PlayerSession> CreateSessionAsync(CreateSessionDto dto, int userId);
    Task<PlayerSession?> GetSessionAsync(int sessionId);
    Task<PlayerSessionDto?> GetSessionDtoAsync(int sessionId);
    Task<Character?> GetCharacterForSessionAsync(int sessionId);
    Task<PlayerSession?> UpdateCharacterAsync(int sessionId, Character character);
    Task<PlayerSession?> MoveToNextActAsync(int sessionId, int nextActNumber);

}