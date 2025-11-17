// Controllers/StoryController.cs
using DragonGame.Data;
using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Controllers
{
    [ApiController]
    [Route("api/story")]
    public class StoryController : ControllerBase
    {
        private readonly IStoryService _storyService;
        private readonly AppDbContext _context;

        public StoryController(IStoryService storyService, AppDbContext context)
        {
            _storyService = storyService;
            _context = context;
        }

        // POST /api/story/start
        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] CreateSessionDto dto)
        {
            try
            {
                Console.WriteLine($"[StoryController] Starting new session for '{dto.CharacterName}'");
                var session = await _storyService.StartStoryAsync(dto);
                return Ok(session);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryController][Start] ERROR: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, "Failed to start story");
            }
        }

        // GET /api/story/session/{sessionId}
        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            try
            {
                Console.WriteLine($"[StoryController] Fetching session {sessionId}");
                var session = await _context.PlayerSessions
                    .Include(s => s.Character)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null) return NotFound();

                var dto = new PlayerSessionDto
                {
                    SessionId = session.SessionId,
                    CharacterName = session.CharacterName,
                    CharacterId = session.CharacterId,
                    Hair = session.Character?.Hair,
                    Face = session.Character?.Face,
                    Outfit = session.Character?.Outfit,
                    PoseId = session.Character?.PoseId,
                    StoryId = session.StoryId,
                    CurrentActNumber = session.CurrentActNumber,
                    IsCompleted = session.IsCompleted
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryController][GetSession] ERROR: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/story/{sessionId}/character
        [HttpGet("{sessionId}/character")]
        public async Task<ActionResult<Character>> GetCharacterForSession(int sessionId)
        {
            try
            {
                Console.WriteLine($"[StoryController] Getting character for session {sessionId}");
                var character = await _storyService.GetCharacterForSessionAsync(sessionId);
                if (character == null) return NotFound();
                return Ok(character);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryController][GetCharacter] ERROR: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        // PUT /api/story/updateCharacter/{sessionId}
        [HttpPut("updateCharacter/{sessionId}")]
        public async Task<IActionResult> UpdateCharacter(int sessionId, [FromBody] UpdateCharacterDto dto)
        {
            try
            {
                Console.WriteLine($"[StoryController] Updating character for session {sessionId}");
                var character = new Character
                {
                    Id = dto.Id,
                    Hair = dto.Hair,
                    Face = dto.Face,
                    Outfit = dto.Outfit,
                    PoseId = dto.PoseId
                };

                var updatedSession = await _storyService.UpdateCharacterAsync(sessionId, character);
                if (updatedSession == null) return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryController][UpdateCharacter] ERROR: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/story/currentAct/{sessionId}
        [HttpGet("currentAct/{sessionId}")]
        public async Task<ActionResult<ActDto>> GetCurrentAct(int sessionId)
        {
            var session = await _context.PlayerSessions
                .Include(s => s.Story!)
                    .ThenInclude(st => st.Acts!)
                        .ThenInclude(a => a.Choices)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound("Session not found");

            if (session.Story?.Acts == null || !session.Story.Acts.Any())
                return NotFound("Story data not loaded");

            var currentAct = session.Story.Acts
                .FirstOrDefault(a => a.ActNumber == session.CurrentActNumber);

            // If somehow the current act is missing → fall back to Act 1
            if (currentAct == null)
            {
                currentAct = session.Story.Acts.FirstOrDefault(a => a.ActNumber == 1);
                if (currentAct != null)
                {
                    session.CurrentActNumber = 1;
                    await _context.SaveChangesAsync();
                }
            }

            if (currentAct == null)
                return NotFound("No valid act found");

            var actDto = new ActDto
            {
                ActNumber = currentAct.ActNumber,
                Text = currentAct.Text,
                Choices = currentAct.Choices.Select(c => new ChoiceDto
                {
                    Text = c.Text,
                    NextActNumber = c.NextActNumber
                }).ToList(),
                IsEnding = currentAct.IsEnding
            };

            return Ok(actDto);
        }

        // POST /api/story/nextAct/{sessionId}
        [HttpPost("nextAct/{sessionId}")]
        public async Task<IActionResult> NextAct(int sessionId, [FromBody] NextActRequest request)
        {
            try
            {
                Console.WriteLine($"[StoryController] Moving session {sessionId} → act {request.NextActNumber}");
                var session = await _storyService.MoveToNextActAsync(sessionId, request.NextActNumber);
                if (session == null) return NotFound();
                return Ok(session);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryController][NextAct] ERROR: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}