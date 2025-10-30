using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonGame.Data;       // For AppDbContext
using DragonGame.Models;     // For PlayerSession, Story, Act
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
public class StoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public StoryController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Start a new session with character
    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] CreateSessionRequest request)
    {
        var session = new PlayerSession
        {
            CharacterName = request.CharacterName,
            CharacterDesignJson = request.CharacterDesignJson,
            StoryId = request.StoryId,
            CurrentActNumber = 1
        };

        _context.PlayerSessions.Add(session);
        await _context.SaveChangesAsync();

        // Return session info (frontend needs SessionId)
        return Ok(new
        {
            sessionId = session.SessionId,
            session.CharacterName,
            session.CharacterDesignJson,
            session.StoryId
        });
    }

    // 2. Get session by ID
    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetSession(int sessionId)
    {
        var session = await _context.PlayerSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return NotFound();

        return Ok(new
        {
            sessionId = session.SessionId,
            session.CharacterName,
            session.CharacterDesignJson,
            session.StoryId,
            session.CurrentActNumber,
            session.IsCompleted
        });
    }

    // 3. Get current act for a session
    [HttpGet("currentAct/{sessionId}")]
    public async Task<IActionResult> GetCurrentAct(int sessionId)
    {
        var session = await _context.PlayerSessions.FirstOrDefaultAsync(s => s.SessionId == sessionId);
        if (session == null) return NotFound();

        var act = await _context.Acts
            .Include(a => a.Choices)
            .FirstOrDefaultAsync(a => a.StoryId == session.StoryId && a.ActNumber == session.CurrentActNumber);

        if (act == null) return NotFound();

        return Ok(new { session, act });
    }

    // 4. Advance to next act
    [HttpPost("nextAct/{sessionId}")]
    public async Task<IActionResult> NextAct(int sessionId, [FromBody] NextActRequest request)
    {
        var session = await _context.PlayerSessions.FirstOrDefaultAsync(s => s.SessionId == sessionId);
        if (session == null) return NotFound();

        session.CurrentActNumber = request.NextActNumber;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            sessionId = session.SessionId,
            session.CurrentActNumber
        });
    }

    // 5. Update character mid-story
    [HttpPost("updateCharacter/{sessionId}")]
    public async Task<IActionResult> UpdateCharacter(int sessionId, [FromBody] UpdateCharacterRequest request)
    {
        var session = await _context.PlayerSessions.FirstOrDefaultAsync(s => s.SessionId == sessionId);
        if (session == null) return NotFound();

        session.CharacterName = request.CharacterName;
        session.CharacterDesignJson = request.CharacterDesignJson;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            sessionId = session.SessionId,
            session.CharacterName,
            session.CharacterDesignJson
        });
    }
}

// Request DTOs
public class CreateSessionRequest
{
    public string CharacterName { get; set; } = "";
    public string CharacterDesignJson { get; set; } = "{}";
    public int StoryId { get; set; }
}

public class NextActRequest
{
    public int NextActNumber { get; set; }
}

public class UpdateCharacterRequest
{
    public string CharacterName { get; set; } = "";
    public string CharacterDesignJson { get; set; } = "{}";
}
