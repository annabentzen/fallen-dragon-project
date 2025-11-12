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

        [HttpPost("start")]
        public async Task<IActionResult> StartStory([FromBody] CreateSessionDto dto)
        {
            try
            {
                var session = await _storyService.StartStoryAsync(dto);
                return Ok(session);
            }
            catch (Exception ex)
            {
                // log full exception to console
                Console.WriteLine(ex);
                return StatusCode(500, ex.Message);
            }
}


        [HttpGet("currentAct/{sessionId}")]
        public async Task<IActionResult> GetCurrentAct(int sessionId)
        {
            var result = await _storyService.GetCurrentActAsync(sessionId);
            if (result == null) return NotFound($"Session {sessionId} or act not found.");

            return Ok(result);
        }

        [HttpPost("nextAct/{sessionId}")]
        public async Task<IActionResult> NextAct(int sessionId, [FromBody] NextActRequest request)
        {
            if (request == null) return BadRequest("Invalid request body.");

            var session = await _storyService.MoveToNextActAsync(sessionId, request.NextActNumber);
            if (session == null) return NotFound($"Session {sessionId} not found.");

            return Ok(session);
        }

        [HttpPut("updateCharacter/{sessionId}")]
        public async Task<IActionResult> UpdateCharacterDesign(int sessionId, [FromBody] Character newDesign)
        {
            if (newDesign == null) return BadRequest("Character design data is required.");

            var updated = await _storyService.UpdateCharacterAsync(sessionId, newDesign);
            if (updated == null) return NotFound($"Session {sessionId} not found.");

            return Ok(new
            {
                message = "Character design updated successfully.",
                updatedDesign = newDesign
            });
        }
    }
}
