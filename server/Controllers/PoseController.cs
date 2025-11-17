using Microsoft.AspNetCore.Mvc;
using DragonGame.Models;
using DragonGame.Services;

namespace DragonGame.Controllers
{
    [ApiController]
[Route("api/poses")]
public class PoseController : ControllerBase
{
    private readonly IPoseService _poseService;

    public PoseController(IPoseService poseService)
    {
        _poseService = poseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterPose>>> GetAllPoses()
    {
        try
        {
            var poses = await _poseService.GetAllPosesAsync();
            return Ok(poses);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PoseController][GetAllPoses] Error: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterPose>> GetPose(int id)
    {
        try
        {
            var pose = await _poseService.GetPoseByIdAsync(id);
            if (pose == null) return NotFound();
            return Ok(pose);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PoseController][GetPose] Error for id={id}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<CharacterPose>> CreatePose(CharacterPose pose)
    {
        try
        {
            await _poseService.AddPoseAsync(pose);
            return CreatedAtAction(nameof(GetPose), new { id = pose.Id }, pose);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PoseController][CreatePose] Error: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePose(int id, CharacterPose pose)
    {
        try
        {
            if (id != pose.Id) return BadRequest();

            var existing = await _poseService.GetPoseByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = pose.Name;
            existing.ImageUrl = pose.ImageUrl;

            await _poseService.UpdatePoseAsync(existing);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PoseController][UpdatePose] Error for id={id}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePose(int id)
    {
        try
        {
            var existing = await _poseService.GetPoseByIdAsync(id);
            if (existing == null) return NotFound();

            await _poseService.DeletePoseAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PoseController][DeletePose] Error for id={id}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }
}

}
