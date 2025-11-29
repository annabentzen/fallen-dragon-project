using DragonGame.Data;
using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Controllers
{
    [ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlayerSessionController : ControllerBase
{
    private readonly PlayerSessionService _service;
    private readonly AppDbContext _context;

    public PlayerSessionController(PlayerSessionService service, AppDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet("{sessionId}")]
    public async Task<ActionResult<PlayerSessionDto>> GetSession(int sessionId)
    {
        try
        {
            Console.WriteLine($"[PlayerSessionController][GetSession] Request for sessionId={sessionId}");

            var session = await _context.PlayerSessions
                .Include(s => s.Character)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
            {
                Console.WriteLine($"[GetSession] Session not found: {sessionId}");
                return NotFound();
            }

            var dto = await _service.GetSessionDtoAsync(sessionId);
            if (dto == null)
            {
                Console.WriteLine($"[GetSession] DTO null for sessionId={sessionId}");
                return NotFound();
            }

            return Ok(dto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PlayerSessionController][GetSession] Error for sessionId={sessionId}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{sessionId}/character")]
    public async Task<ActionResult<Character>> GetCharacterForSession(int sessionId)
    {
        try
        {
            var session = await _context.PlayerSessions
                .Include(s => s.Character)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session?.Character == null)
            {
                Console.WriteLine($"[GetCharacterForSession] No session or character found: sessionId={sessionId}");
                return NotFound();
            }

            return Ok(session.Character);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PlayerSessionController][GetCharacterForSession] Error: {ex}");
            return StatusCode(500, ex.Message);
        }
    }
}

}
