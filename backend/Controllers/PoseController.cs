using Microsoft.AspNetCore.Mvc;
using DragonGame.Models;
using DragonGame.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonGame.Controllers
{
    [ApiController]
    [Route("api/poses")]
    public class PoseController : ControllerBase
    {
        private readonly ICharacterPoseRepository _poseRepository;

        public PoseController(ICharacterPoseRepository poseRepository)
        {
            _poseRepository = poseRepository;
        }

        // ✅ GET: api/poses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterPose>>> GetAllPoses()
        {
            var poses = await _poseRepository.GetAllPosesAsync();
            return Ok(poses);
        }

        // ✅ GET: api/poses/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterPose>> GetPose(int id)
        {
            var pose = await _poseRepository.GetByIdAsync(id);
            if (pose == null)
                return NotFound();

            return Ok(pose);
        }

        // ✅ POST: api/poses
        [HttpPost]
        public async Task<ActionResult<CharacterPose>> CreatePose(CharacterPose pose)
        {
            await _poseRepository.AddAsync(pose);
            return CreatedAtAction(nameof(GetPose), new { id = pose.Id }, pose);
        }

        // ✅ PUT: api/poses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePose(int id, CharacterPose pose)
        {
            if (id != pose.Id)
                return BadRequest();

            var existing = await _poseRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = pose.Name;
            existing.ImageUrl = pose.ImageUrl;

            await _poseRepository.UpdateAsync(existing);
            return NoContent();
        }

        // ✅ DELETE: api/poses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePose(int id)
        {
            var pose = await _poseRepository.GetByIdAsync(id);
            if (pose == null)
                return NotFound();

            await _poseRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
