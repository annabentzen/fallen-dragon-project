using DragonGame.Data;
using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlayerSessionController : ControllerBase
{
    private readonly IPlayerSessionService _service;
    private readonly AppDbContext _context;
    private readonly ILogger<PlayerSessionController> _logger;

    public PlayerSessionController(
        IPlayerSessionService service, 
        AppDbContext context,
        ILogger<PlayerSessionController> logger)
    {
        _service = service;
        _context = context;
        _logger = logger;
    }

    [HttpGet("{sessionId}")]
    public async Task<ActionResult<PlayerSessionDto>> GetSession(int sessionId)
    {
        var dto = await _service.GetSessionDtoAsync(sessionId);

        if (dto == null)
        {
            _logger.LogWarning("Session not found: {SessionId}", sessionId);
            return NotFound(new { message = "Session not found" });
        }

        return Ok(dto);
    }

    [HttpGet("{sessionId}/character")]
    public async Task<ActionResult<Character>> GetCharacterForSession(int sessionId)
    {
        var session = await _context.PlayerSessions
            .Include(s => s.Character)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session?.Character == null)
        {
            _logger.LogWarning("Character not found for session: {SessionId}", sessionId);
            return NotFound(new { message = "Character not found" });
        }

        return Ok(session.Character);
    }

   [HttpDelete("{sessionId}")]
public async Task<ActionResult> DeleteSession(int sessionId)
{
    // Debug: Log all claims
    _logger.LogInformation("All claims: {Claims}", 
        string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
    
    var userIdClaim = User.FindFirst("userId")?.Value 
        ?? User.FindFirst("sub")?.Value 
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    
    _logger.LogInformation("Extracted userId: {UserId}, sessionId requested: {SessionId}", 
        userIdClaim, sessionId);
    
    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
    {
        _logger.LogWarning("Unauthorized delete attempt for session {SessionId}", sessionId);
        return Unauthorized(new { message = "Invalid user token" });
    }

    // Debug: Check if session exists at all
    var sessionExists = await _context.PlayerSessions.AnyAsync(s => s.SessionId == sessionId);
    var sessionWithUser = await _context.PlayerSessions.AnyAsync(s => s.SessionId == sessionId && s.UserId == userId);
    _logger.LogInformation("Session {SessionId} exists: {Exists}, belongs to user {UserId}: {BelongsToUser}", 
        sessionId, sessionExists, userId, sessionWithUser);

    var deleted = await _service.DeleteSessionAsync(sessionId, userId);
    
    if (!deleted)
    {
        _logger.LogWarning("Delete failed for session {SessionId}, user {UserId}", sessionId, userId);
        return NotFound(new { message = "Session not found or access denied" });
    }

    _logger.LogInformation("Session {SessionId} deleted by user {UserId}", sessionId, userId);
    return NoContent();
}
}