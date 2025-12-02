using System.Security.Claims;
using DragonGame.Data;
using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Controllers;

[ApiController]
[Route("api/story")]
[Authorize]
public class StoryController : ControllerBase
{
    private readonly IStoryService _storyService;
    private readonly AppDbContext _context;
    private readonly IPlayerSessionService _sessionService;
    private readonly ILogger<StoryController> _logger;

    public StoryController(
        IStoryService storyService, 
        IPlayerSessionService sessionService, 
        AppDbContext context,
        ILogger<StoryController> logger)
    {
        _storyService = storyService;
        _sessionService = sessionService;
        _context = context;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] CreateSessionDto dto)
    {
        var userId = GetCurrentUserId();
        
        _logger.LogInformation(
            "User {UserId} starting session with character {CharacterName}", 
            userId, 
            dto.CharacterName);

        var session = await _sessionService.CreateSessionAsync(dto, userId);

        _logger.LogInformation(
            "Session {SessionId} created for user {UserId}", 
            session.SessionId, 
            userId);

        return Ok(session);
    }

    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetSession(int sessionId)
    {
        var session = await _context.PlayerSessions
            .Include(s => s.Character)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null)
        {
            _logger.LogWarning("Session not found: {SessionId}", sessionId);
            return NotFound(new { message = "Session not found" });
        }

        var dto = new PlayerSessionDto
        {
            SessionId = session.SessionId,
            CharacterName = session.CharacterName,
            CharacterId = session.CharacterId,
            Head = session.Character?.Head,
            Body = session.Character?.Body,
            PoseId = session.Character?.PoseId,
            StoryId = session.StoryId,
            CurrentActNumber = session.CurrentActNumber,
            IsCompleted = session.IsCompleted
        };

        return Ok(dto);
    }

    [HttpGet("{sessionId}/character")]
    public async Task<ActionResult<Character>> GetCharacterForSession(int sessionId)
    {
        var character = await _storyService.GetCharacterForSessionAsync(sessionId);
        
        if (character == null)
        {
            _logger.LogWarning("Character not found for session: {SessionId}", sessionId);
            return NotFound(new { message = "Character not found" });
        }
        
        return Ok(character);
    }

    [HttpPut("updateCharacter/{sessionId}")]
    public async Task<IActionResult> UpdateCharacter(int sessionId, [FromBody] UpdateCharacterDto dto)
    {
        var character = new Character
        {
            Id = dto.Id,
            Head = dto.Head,
            Body = dto.Body,
            PoseId = dto.PoseId
        };

        var updatedSession = await _storyService.UpdateCharacterAsync(sessionId, character);
        
        if (updatedSession == null)
        {
            _logger.LogWarning("Update failed - session not found: {SessionId}", sessionId);
            return NotFound(new { message = "Session not found" });
        }

        _logger.LogInformation("Character updated for session {SessionId}", sessionId);
        return Ok();
    }

    [HttpGet("currentAct/{sessionId}")]
    public async Task<ActionResult<ActDto>> GetCurrentAct(int sessionId)
    {
        var session = await _context.PlayerSessions
            .Include(s => s.Story!)
                .ThenInclude(st => st.Acts!)
                    .ThenInclude(a => a.Choices)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null)
        {
            _logger.LogWarning("Session not found: {SessionId}", sessionId);
            return NotFound(new { message = "Session not found" });
        }

        if (session.Story?.Acts == null || !session.Story.Acts.Any())
        {
            _logger.LogWarning("Story data not loaded for session: {SessionId}", sessionId);
            return NotFound(new { message = "Story data not loaded" });
        }

        var currentAct = session.Story.Acts
            .FirstOrDefault(a => a.ActNumber == session.CurrentActNumber);

        // Fallback to Act 1 if current act is missing (data integrity recovery)
        if (currentAct == null)
        {
            currentAct = session.Story.Acts.FirstOrDefault(a => a.ActNumber == 1);
            if (currentAct != null)
            {
                _logger.LogWarning(
                    "Act {ActNumber} not found for session {SessionId}, falling back to Act 1",
                    session.CurrentActNumber,
                    sessionId);
                    
                session.CurrentActNumber = 1;
                await _context.SaveChangesAsync();
            }
        }

        if (currentAct == null)
        {
            _logger.LogError("No valid act found for session: {SessionId}", sessionId);
            return NotFound(new { message = "No valid act found" });
        }

        var actDto = new ActDto
        {
            ActNumber = currentAct.ActNumber,
            Text = currentAct.Text,
            Choices = [.. currentAct.Choices.Select(c => new ChoiceDto
            {
                Text = c.Text,
                NextActNumber = c.NextActNumber
            })],
            IsEnding = currentAct.IsEnding
        };

        return Ok(actDto);
    }

    [HttpPost("nextAct/{sessionId}")]
    public async Task<IActionResult> NextAct(int sessionId, [FromBody] NextActRequest request)
    {
        _logger.LogInformation(
            "Session {SessionId} advancing to act {NextActNumber}", 
            sessionId, 
            request.NextActNumber);

        var session = await _storyService.MoveToNextActAsync(sessionId, request.NextActNumber);
        
        if (session == null)
        {
            _logger.LogWarning("Failed to advance - session not found: {SessionId}", sessionId);
            return NotFound(new { message = "Session not found" });
        }

        return Ok(session);
    }
}