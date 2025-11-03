using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DragonGame.Data;
using DragonGame.Models;

namespace DragonGame.Controllers
{
    [ApiController]
    [Route("api/story")]
    public class StoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StoryController(AppDbContext context)
        {
            _context = context;
        }

        // Start a new story session
        [HttpPost("start")]
        public async Task<ActionResult<PlayerSession>> StartStory([FromBody] PlayerSession session)
        {
            if (session == null)
                return BadRequest("Session data is required.");

            session.CurrentActNumber = 1;
            session.IsCompleted = false;

            _context.PlayerSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(session);
        }

        // Get session by ID
        [HttpGet("session/{id}")]
        public async Task<ActionResult<PlayerSession>> GetSession(int id)
        {
            var session = await _context.PlayerSessions.FindAsync(id);
            if (session == null) return NotFound();
            return Ok(session);
        }

        // Get current act for a session
        [HttpGet("currentAct/{sessionId}")]
        public async Task<ActionResult<object>> GetCurrentAct(int sessionId)
        {
            var session = await _context.PlayerSessions.FindAsync(sessionId);
            if (session == null) return NotFound();

            var act = await _context.Acts
                .Include(a => a.Choices)
                .FirstOrDefaultAsync(a => a.ActNumber == session.CurrentActNumber);

            if (act == null) return NotFound();

            // Make sure Choices is always an array
            var choicesList = act.Choices.ToList();

            return Ok(new { session, act = new { act.ActNumber, act.Text, choices = choicesList } });
        }

        // Move to next act
        [HttpPost("nextAct/{sessionId}")]
        public async Task<ActionResult<PlayerSession>> NextAct(int sessionId, [FromBody] int nextActNumber)
        {
            var session = await _context.PlayerSessions.FindAsync(sessionId);
            if (session == null) return NotFound();

            if (nextActNumber <= 0)
                session.IsCompleted = true; // Special ending
            else
                session.CurrentActNumber = nextActNumber;

            await _context.SaveChangesAsync();
            return Ok(session);
        }
    }
}
