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
    private readonly PlayerSessionService _service;
    private readonly AppDbContext _context;
    private readonly ILogger<PlayerSessionController> _logger;

    public PlayerSessionController(
        PlayerSessionService service, 
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
}