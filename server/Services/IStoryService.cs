using DragonGame.Dtos;
using DragonGame.Models;

namespace DragonGame.Services
{
    public interface IStoryService
    {
        Task<PlayerSession> StartStoryAsync(CreateSessionDto dto);
        Task<PlayerSession?> GetSessionByIdAsync(int id);
        Task<object?> GetCurrentActAsync(int sessionId);
        Task<PlayerSession?> MoveToNextActAsync(int sessionId, int nextActNumber);
        Task<PlayerSession?> UpdateCharacterAsync(int sessionId, Character newDesign);
    }
}
