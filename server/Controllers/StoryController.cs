using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DragonGame.Data;
using DragonGame.Models;
using System.Text.Json;
using DragonGame.Dtos;

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

        // --- Start a new story session ---
        [HttpPost("start")]
        public async Task<ActionResult<PlayerSession>> StartStory([FromBody] CreateSessionDto dto)
        {
            if (dto == null)
                return BadRequest("Session data is required.");

            var session = new PlayerSession
            {
                CharacterName = dto.CharacterName,
                StoryId = dto.StoryId,
                CharacterDesignJson = JsonSerializer.Serialize(dto.CharacterDesign),
                CurrentActNumber = 1,
                IsCompleted = false
            };

            _context.PlayerSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(session);
        }

        // --- Get session by ID ---
        [HttpGet("session/{id}")]
        public async Task<ActionResult<PlayerSession>> GetSession(int id)
        {
            var session = await _context.PlayerSessions.FindAsync(id);
            if (session == null) return NotFound();
            return Ok(session);
        }

        // --- Get current act for a session ---
        [HttpGet("currentAct/{sessionId}")]
        public IActionResult GetCurrentAct(int sessionId)
        {
            var session = _context.PlayerSessions.FirstOrDefault(s => s.SessionId == sessionId);
            if (session == null) return NotFound($"Session {sessionId} not found.");

            var act = _context.Acts
                .Include(a => a.Choices)
                .FirstOrDefault(a => a.ActNumber == session.CurrentActNumber && a.StoryId == session.StoryId);

            if (act == null) return NotFound("Act not found for current session.");

            // Parse CharacterDesignJson safely
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
                    choices = act.Choices.Select(c => new
                    {
                        c.ChoiceId,
                        c.Text,
                        c.ActId,
                        c.NextActNumber
                    }).ToList()
                }
            });
        }

        // --- Move to next act ---
        public class NextActRequest
        {
            public int NextActNumber { get; set; }
        }

        [HttpPost("nextAct/{sessionId}")]
        public async Task<ActionResult<PlayerSession>> NextAct(int sessionId, [FromBody] NextActRequest request)
        {
            var session = await _context.PlayerSessions.FindAsync(sessionId);
            if (session == null)
                return NotFound();

            if (request == null)
                return BadRequest("Invalid request body.");

            if (request.NextActNumber <= 0)
                session.IsCompleted = true;
            else
                session.CurrentActNumber = request.NextActNumber;

            await _context.SaveChangesAsync();
            return Ok(session);
        }

        // --- Update character design mid-story ---
        [HttpPut("updateCharacter/{sessionId}")]
        public async Task<IActionResult> UpdateCharacterDesign(int sessionId, [FromBody] CharacterDesign newDesign)
        {
            if (newDesign == null)
                return BadRequest("Character design data is required.");

            var session = await _context.PlayerSessions.FindAsync(sessionId);
            if (session == null)
                return NotFound($"Session {sessionId} not found.");

            session.CharacterDesignJson = JsonSerializer.Serialize(newDesign);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Character design updated successfully.",
                updatedDesign = newDesign
            });
        }
    }
}
