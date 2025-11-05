using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DragonGame.Data;
using DragonGame.Models;
using System.Text.Json;

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

            // Ensure consistent initialization
            session.CurrentActNumber = 1;
            session.IsCompleted = false;

            // If CharacterDesign is an object, serialize it to JSON
            if (session.CharacterDesign != null && string.IsNullOrEmpty(session.CharacterDesignJson))
            {
                session.CharacterDesignJson = JsonSerializer.Serialize(session.CharacterDesign);
            }

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

        // Get current act for a session (clean version)
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

            // Choices are returned as a plain array
            var cleanChoices = act.Choices.Select(c => new
            {
                c.ChoiceId,
                c.Text,
                c.ActId,
                c.NextActNumber
            }).ToList();

            var cleanAct = new
            {
                act.ActNumber,
                act.Text,
                choices = cleanChoices
            };

            // Include CharacterDesign as an object if JSON exists
            object? designObj = null;
            if (!string.IsNullOrEmpty(session.CharacterDesignJson))
            {
                try
                {
                    designObj = JsonSerializer.Deserialize<object>(session.CharacterDesignJson);
                }
                catch
                {
                    designObj = session.CharacterDesignJson; // fallback
                }
            }

            return Ok(new
            {
                session.SessionId,
                session.CharacterName,
                characterDesign = designObj,
                session.CurrentActNumber,
                session.IsCompleted,
                act = cleanAct
            });
        }

        // Move to next act
        public class NextActRequest
        {
            public int NextActNumber { get; set; }
        }

        [HttpPost("nextAct/{sessionId}")]
        public async Task<ActionResult<PlayerSession>> NextAct(
            int sessionId,
            [FromBody] NextActRequest request)
        {
            var session = await _context.PlayerSessions.FindAsync(sessionId);
            if (session == null)
                return NotFound();

            // Handle request correctly
            if (request == null)
                return BadRequest("Invalid request body.");

            // Update act number or mark as completed
            if (request.NextActNumber <= 0)
                session.IsCompleted = true;
            else
                session.CurrentActNumber = request.NextActNumber;

            await _context.SaveChangesAsync();
            return Ok(session);
        }
    }
}
