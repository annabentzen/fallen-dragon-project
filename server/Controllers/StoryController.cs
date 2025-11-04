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
        public IActionResult GetCurrentAct(int sessionId)
        {
            var session = _context.PlayerSessions
                .FirstOrDefault(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound();

            var act = _context.Acts
                .Include(a => a.Choices)
                .FirstOrDefault(a => a.StoryId == session.StoryId && a.ActNumber == session.CurrentActNumber);

            if (act == null)
                return NotFound();

            // Map Choices to a simple array
            var cleanChoices = act.Choices.Select(c => new
            {
                c.ChoiceId,
                c.Text,
                c.ActId,
                c.NextActNumber
            }).ToList(); // This is already a plain array

            var cleanAct = new
            {
                act.ActNumber,
                act.Text,
                choices = cleanChoices // plain array
            };

            return Ok(new
            {
                session,
                act = cleanAct
            });
        }



        // Move to next act
        public class NextActRequest { public int NextActNumber { get; set; } }
        [HttpPost("nextAct/{sessionId}")]
        public async Task<ActionResult<PlayerSession>> NextAct(int sessionId, [FromBody] NextActRequest request)
        {
            var session = await _context.PlayerSessions.FindAsync(sessionId);
            if (session == null) return NotFound();

            if (request.NextActNumber <= 0)
                session.IsCompleted = true;
            else
                session.CurrentActNumber = request.NextActNumber;

            await _context.SaveChangesAsync();
            return Ok(session);
        }

    }
}
