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


        // get current act for session
        [HttpGet("currentAct/{sessionId}")]
        public IActionResult GetCurrentAct(int sessionId)
        {
            var session = _context.PlayerSessions
                //.Include(s => s.CharacterDesign) // ❌ REMOVE this, CharacterDesign is not an entity
                .FirstOrDefault(s => s.SessionId == sessionId);

            if (session == null) return NotFound();

            var act = _context.Acts
                .Include(a => a.Choices)
                .FirstOrDefault(a => a.ActNumber == session.CurrentActNumber && a.StoryId == session.StoryId);

            if (act == null) return NotFound();

            // Parse CharacterDesignJson to object safely
            CharacterDesign parsedDesign;
            if (!string.IsNullOrEmpty(session.CharacterDesignJson))
            {
                try
                {
                    parsedDesign = JsonSerializer.Deserialize<CharacterDesign>(session.CharacterDesignJson);
                }
                catch
                {
                    parsedDesign = new CharacterDesign(); // fallback
                }
            }
            else
            {
                parsedDesign = new CharacterDesign(); 
            }

            // Return session + act
            return Ok(new
            {
                session = new
                {
                    session.SessionId,
                    session.CharacterName,
                    CharacterDesign = parsedDesign,
                    session.StoryId,
                    session.CurrentActNumber,
                    session.IsCompleted
                },
                act = new
                {
                    act.ActNumber,
                    act.Text,
                    choices = act.Choices.Select(c => new {
                        c.ChoiceId,
                        c.Text,
                        c.ActId,
                        c.NextActNumber
                    }).ToList()
                }
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
