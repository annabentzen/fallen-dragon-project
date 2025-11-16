using DragonGame.Data;
using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PlayerSessionController : ControllerBase
    {
        private readonly PlayerSessionService _service;

        private readonly AppDbContext _context;

        public PlayerSessionController(PlayerSessionService service)
        {
            _service = service;
        }

        // GET api/playersession/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerSessionDto>> GetSession(int id)
        {
            var dto = await _service.GetSessionDtoAsync(id);
            if (dto == null) return NotFound();

            return Ok(dto);
        }

        // GET api/playersession/{sessionId}/character
        [HttpGet("{sessionId}/character")]
        public async Task<ActionResult<Character>> GetCharacterForSession(int sessionId)
        {
            var session = await _context.PlayerSessions
                .Include(s => s.Character)  
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null || session.Character == null)
                return NotFound();

            return Ok(session.Character);
        }

    }
}