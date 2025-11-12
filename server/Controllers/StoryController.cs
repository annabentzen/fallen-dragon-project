using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DragonGame.Data;
using DragonGame.Models;
using System.Text.Json;
using DragonGame.Dtos;

namespace DragonGame.Controllers
{
    /// <summary>
    /// REST API Controller for managing story sessions and game progression. Handles starting stories, tracking player progress through acts, and managing choices. Uses [ApiController] attribute for automatic model validation and API-specific behaviors.
    /// </summary>
    [ApiController]
    [Route("api/story")] // Base route: all endpoints start with /api/story
    public class StoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        /// <summary>
        /// Initializes a new instance of StoryController with db context.
        /// </summary>
        /// <param name="context">EF db context for data access</param>
        public StoryController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Starts a new story session for a player. Creates a playing session with character design stores as JSON.
        /// POST: api/story/start
        /// </summary>
        /// <param name="dto">Data transfer object containing session details</param>
        /// <returns>The created player session object</returns>
        [HttpPost("start")]
        public async Task<ActionResult<PlayerSession>> StartStory([FromBody] CreateSessionDto dto)
        {
            if (dto == null) return BadRequest("Session data is required.");

            // check data annotations validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return validation errors
            }

            // create new player session with initial data
            var session = new PlayerSession
            {
                CharacterName = dto.CharacterName,
                StoryId = dto.StoryId,
                // Serialize character design to JSON for storage
                CharacterDesignJson = JsonSerializer.Serialize(dto.CharacterDesign),
                CurrentActNumber = 1,
                IsCompleted = false
            };

            // Save to database
            _context.PlayerSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(session);
        }

        /// <summary>
        /// Gets a player session by its ID.
        /// GET: api/story/session/{id}
        /// </summary>
        /// <param name="id">The ID of the session</param>
        /// <returns>The requested player session</returns>
        [HttpGet("session/{id}")]
        public async Task<ActionResult<PlayerSession>> GetSession(int id)
        {
            var session = await _context.PlayerSessions.FindAsync(id);
            if (session == null) return NotFound();
            return Ok(session);
        }


        /// <summary>
        /// Gets the current act for a session including choices. Also deserializes the character design for display in the UI.
        /// GET: api/story/currentAct/{sessionId}
        /// </summary>
        /// <param name="sessionId">The ID of the session</param>
        /// <returns>An object containing session data (with parsed character design) and current act data (with choices)</returns>
        [HttpGet("currentAct/{sessionId}")]
        public IActionResult GetCurrentAct(int sessionId)
        {
            // find the player's session
            var session = _context.PlayerSessions
                .FirstOrDefault(s => s.SessionId == sessionId);

            if (session == null) return NotFound();

            // find the current act with its choices
            var act = _context.Acts
                .Include(a => a.Choices) // load choices for the act
                .FirstOrDefault(a => a.ActNumber == session.CurrentActNumber && a.StoryId == session.StoryId);

            if (act == null) return NotFound();

            // deserialize character design JSON back to object for frontend
            CharacterDesign parsedDesign;
            if (!string.IsNullOrEmpty(session.CharacterDesignJson))
            {
                try
                {
                    // attempt to parse JSON to CharacterDesign object
                    parsedDesign = JsonSerializer.Deserialize<CharacterDesign>(session.CharacterDesignJson) ?? new CharacterDesign();
                }
                catch
                {
                    // fallback to empty design if JSON parsing fails
                    parsedDesign = new CharacterDesign();
                }
            }
            else
            {
                // fallback if JSON is null or empty
                parsedDesign = new CharacterDesign();
            }

            // return anonymous object with session and act data
            // so frontend gets both in one response
            return Ok(new
            {
                session = new
                {
                    session.SessionId,
                    session.CharacterName,
                    CharacterDesign = parsedDesign, // parsed design object
                    session.StoryId,
                    session.CurrentActNumber,
                    session.IsCompleted
                },
                act = new
                {
                    act.ActNumber,
                    act.Text, // the act's story text
                    choices = act.Choices.Select(c => new
                    {
                        c.ChoiceId,
                        c.Text, // the choice text shown to player
                        c.ActId,
                        c.NextActNumber // which act to go to if this choice is selected
                    }).ToList()
                }
            });
        }

        /// <summary>
        /// advances a player session to the next act or marks it as completed
        /// POST: api/story/nextAct/{sessionId}
        /// </summary>
        /// <param name="sessionId">The ID of the session to update</param>
        /// <param name="request">object containing the next act number to move to</param>
        /// <returns>The updated player session</returns>
        [HttpPost("nextAct/{sessionId}")]
        public async Task<ActionResult<PlayerSession>> NextAct(
            int sessionId,
            [FromBody] NextActRequest request)
        {
            // Find the session by ID
            var session = await _context.PlayerSessions.FindAsync(sessionId);
            if (session == null)
                return NotFound();

            // validate request body
            if (request == null)
                return BadRequest("Invalid request body.");

            // Update act number or mark as completed
            if (request.NextActNumber <= 0)
                // NextActNumber of 0 or negative indicate story ending
                session.IsCompleted = true;
            else
                // move to the specified next act
                session.CurrentActNumber = request.NextActNumber;

            // Save changes to database
            await _context.SaveChangesAsync();

            return Ok(session);
        }
    }
}
