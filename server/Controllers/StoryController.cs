using Microsoft.AspNetCore.Mvc;
using DragonGame.Services;
using DragonGame.Dtos;
using DragonGame.Models;

namespace DragonGame.Controllers
{
    [ApiController]
[Route("api/[controller]")]
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
        try
        {
            var session = await _storyService.GetSessionByIdAsync(sessionId);
            if (session == null) return NotFound($"Session {sessionId} not found.");
            return Ok(session);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StoryController][GetSession] Error for sessionId={sessionId}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("currentAct/{sessionId}")]
    public async Task<IActionResult> GetCurrentAct(int sessionId)
    {
        try
        {
            var result = await _storyService.GetCurrentActAsync(sessionId);
            if (result == null) return NotFound($"Session {sessionId} or act not found.");

            dynamic dyn = result;
            var actNumber = dyn?.act?.ActNumber ?? -1;
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StoryController][GetCurrentAct] Error for sessionId={sessionId}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("nextAct/{sessionId}")]
    public async Task<IActionResult> NextAct(int sessionId, [FromBody] NextActRequest request)
    {
        if (request == null) return BadRequest("Invalid request body.");
        try
        {
            var session = await _storyService.MoveToNextActAsync(sessionId, request.NextActNumber);
            if (session == null) return NotFound($"Session {sessionId} not found.");
            return Ok(session);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StoryController][NextAct] Error for sessionId={sessionId}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("updateCharacter/{sessionId}")]
    public async Task<IActionResult> UpdateCharacter(int sessionId, [FromBody] UpdateCharacterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

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
            Console.WriteLine($"[StoryController][UpdateCharacter] Error for sessionId={sessionId}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }
}

}
