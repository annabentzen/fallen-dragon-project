using Microsoft.AspNetCore.Mvc;
using DragonGame.Models;
using DragonGame.Services;

namespace DragonGame.Controllers;

[ApiController]
[Route("api/poses")]
public class PoseController : ControllerBase
{
    private readonly IPoseService _poseService;
    private readonly ILogger<PoseController> _logger;

    public PoseController(IPoseService poseService, ILogger<PoseController> logger)
    {
        _poseService = poseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterPose>>> GetAllPoses()
    {
        var poses = await _poseService.GetAllPosesAsync();
        return Ok(poses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterPose>> GetPose(int id)
    {
        var pose = await _poseService.GetPoseByIdAsync(id);
        
        if (pose == null)
        {
            _logger.LogWarning("Pose not found: {PoseId}", id);
            return NotFound(new { message = "Pose not found" });
        }
        
        return Ok(pose);
    }

    [HttpPost]
    public async Task<ActionResult<CharacterPose>> CreatePose([FromBody] CharacterPose pose)
    {
        await _poseService.AddPoseAsync(pose);
        
        _logger.LogInformation("Pose created: {PoseId} ({PoseName})", pose.Id, pose.Name);
        
        return CreatedAtAction(nameof(GetPose), new { id = pose.Id }, pose);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePose(int id, [FromBody] CharacterPose pose)
    {
        if (id != pose.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        var existing = await _poseService.GetPoseByIdAsync(id);
        
        if (existing == null)
        {
            _logger.LogWarning("Update failed - pose not found: {PoseId}", id);
            return NotFound(new { message = "Pose not found" });
        }

        existing.Name = pose.Name;
        existing.ImageUrl = pose.ImageUrl;
        await _poseService.UpdatePoseAsync(existing);

        _logger.LogInformation("Pose updated: {PoseId}", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePose(int id)
    {
        var existing = await _poseService.GetPoseByIdAsync(id);
        
        if (existing == null)
        {
            _logger.LogWarning("Delete failed - pose not found: {PoseId}", id);
            return NotFound(new { message = "Pose not found" });
        }

        await _poseService.DeletePoseAsync(id);
        
        _logger.LogInformation("Pose deleted: {PoseId}", id);
        return NoContent();
    }
}