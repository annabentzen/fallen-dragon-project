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
        public async Task<ActionResult<PlayerSession>> StartStory([FromBody] JsonElement sessionData)
        {
            if (sessionData.ValueKind != JsonValueKind.Object)
                return BadRequest("Session data is required.");

            // Character name
            string characterName = sessionData.TryGetProperty("characterName", out var nameProp)
                ? nameProp.GetString() ?? "Unnamed Hero"
                : "Unnamed Hero";

            // Story ID
            int storyId = sessionData.TryGetProperty("storyId", out var storyProp)
                ? storyProp.GetInt32()
                : 1;

            // CharacterDesign
            if (!sessionData.TryGetProperty("characterDesign", out var charDesign))
                return BadRequest("Character design is missing.");

            string charDesignJson = charDesign.GetRawText(); // store JSON as string

            var session = new PlayerSession
            {
                CharacterName = characterName,
                StoryId = storyId,
                CharacterDesignJson = charDesignJson,
                CurrentActNumber = 1,
                IsCompleted = false
            };

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
                    parsedDesign = JsonSerializer.Deserialize<CharacterDesign>(session.CharacterDesignJson) ?? new CharacterDesign();
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
