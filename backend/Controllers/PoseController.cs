using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DragonGame.Data;
using DragonGame.Models;
using DragonGame.Repositories;

namespace DragonGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoseController : ControllerBase
    {
        private readonly ICharacterPoseRepository _poseRepository;
        private readonly ILogger<PoseController> _logger;

        public PoseController(ICharacterPoseRepository poseRepository, ILogger<PoseController> logger)
        {
            _poseRepository = poseRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterPose>>> GetPoses()
        {
            var poses = await _poseRepository.GetAllAsync();
            return Ok(poses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterPose>> GetPose(int id)
        {
            var pose = await _poseRepository.GetByIdAsync(id);

            if (pose == null)
            {
                return NotFound();
            }

            return Ok(pose);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterPose>> CreatePose(CharacterPose pose)
        {
            await _poseRepository.AddAsync(pose);
            return CreatedAtAction(nameof(GetPose), new { id = pose.Id }, pose);
            return CreatedAtAction(nameof(GetPose), new { id = pose.Id }, pose);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePose(int id, CharacterPose pose)
        {
            if (id != pose.Id)
                return BadRequest();

            await _poseRepository.UpdateAsync(pose);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePose(int id)
        {
            await _poseRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}