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
            var poses = await _poseService.GetAllPosesAsync();
            Console.WriteLine($"Returning {poses.Count()} poses"); //logging
            return Ok(poses.ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterPose>> GetPose(int id)
        {
            var pose = await _poseService.GetPoseByIdAsync(id);
            if (pose == null) return NotFound();
            return Ok(pose);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterPose>> CreatePose(CharacterPose pose)
        {
            await _poseService.AddPoseAsync(pose);
            return CreatedAtAction(nameof(GetPose), new { id = pose.Id }, pose);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePose(int id, CharacterPose pose)
        {
            if (id != pose.Id) return BadRequest();

            var existing = await _poseService.GetPoseByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = pose.Name;
            existing.ImageUrl = pose.ImageUrl;

            await _poseService.UpdatePoseAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePose(int id)
        {
            var pose = await _poseService.GetPoseByIdAsync(id);
            if (pose == null) return NotFound();

            await _poseService.DeletePoseAsync(id);
            return NoContent();
        }
    }
}
