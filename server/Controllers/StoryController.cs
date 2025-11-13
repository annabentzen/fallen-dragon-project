using Microsoft.AspNetCore.Mvc;
using DragonGame.Services;
using DragonGame.Dtos;
using DragonGame.Models;

namespace DragonGame.Controllers
{
    [ApiController]
    [Route("api/story")]
    public class StoryController : ControllerBase
    {
        private readonly IStoryService _storyService;

        public StoryController(IStoryService storyService)
        {
            _storyService = storyService;
        }


        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            Console.WriteLine($"[StoryController] GetSession called with sessionId={sessionId}");
            var session = await _storyService.GetSessionByIdAsync(sessionId);
            if (session == null)
            {
                Console.WriteLine($"[StoryController] Session {sessionId} not found.");
                return NotFound($"Session {sessionId} not found.");
            }

            Console.WriteLine($"[StoryController] Found session {sessionId}");
            return Ok(session);
        }



        [HttpPost("start")]
        public async Task<IActionResult> StartStory([FromBody] CreateSessionDto dto)
        {
            try
            {
                Console.WriteLine($"[StoryController] StartStory called with characterName={dto.CharacterName}");
                var session = await _storyService.StartStoryAsync(dto);

                Console.WriteLine($"[StoryController] StartStory started sessionId={session.SessionId}");
                return Ok(session);
            }
            catch (Exception ex)
            {
                // log full exception to console
                Console.WriteLine($"[StoryController] StartStory failed " + ex);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("currentAct/{sessionId}")]
        public async Task<IActionResult> GetCurrentAct(int sessionId)
        {
            Console.WriteLine($"[StoryController] GetCurrentAct called for session {sessionId}");
            var result = await _storyService.GetCurrentActAsync(sessionId);

            if (result == null)
            {
                Console.WriteLine($"[StoryController] No act found for session {sessionId}");
                return NotFound($"Session {sessionId} or act not found.");
            }
            dynamic dyn = result;
            var actNumber = dyn?.act?.ActNumber ?? -1;

            Console.WriteLine($"[StoryController] Returning act {actNumber} for session {sessionId}");
            return Ok(result);
        }





        [HttpPost("nextAct/{sessionId}")]
        public async Task<IActionResult> NextAct(int sessionId, [FromBody] NextActRequest request)
        {
            Console.WriteLine($"[StoryController] NextAct called for session {sessionId} to move to act {request?.NextActNumber}");
            if (request == null) return BadRequest("Invalid request body.");

            var session = await _storyService.MoveToNextActAsync(sessionId, request.NextActNumber);
            if (session == null) return NotFound($"Session {sessionId} not found.");

            Console.WriteLine($"[StoryController] Session {sessionId} moved to act {session.CurrentActNumber}");
            return Ok(session);
        }

        [HttpPut("updateCharacter/{sessionId}")]
        public async Task<IActionResult> UpdateCharacter(int sessionId, [FromBody] UpdateCharacterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO to Character entity
            var character = new Character
            {
                Hair = dto.Hair,
                Face = dto.Face,
                Outfit = dto.Outfit,
                PoseId = dto.PoseId
            };

            try
            {
                await _storyService.UpdateCharacterAsync(sessionId, character);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




    }
}
