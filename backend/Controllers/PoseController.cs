using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DragonGame.Data;
using DragonGame.Models;

public class PoseController : ControllerBase
{
    private readonly DragonGameDbContext _context;

    public PoseController(DragonGameDbContext context)
    {
        _context = context;
    }

    [HttpGet("api/poses")]
    public async Task<ActionResult<IEnumerable<CharacterPose>>> GetPoses()
    {
        return await _context.CharacterPoses.ToListAsync();
    }

    [HttpGet("api/poses/{id}")]
    public async Task<ActionResult<CharacterPose>> GetPose(int id)
    {
        var pose = await _context.CharacterPoses.FindAsync(id);

        if (pose == null)
        {
            return NotFound();
        }

        return pose;
    }

    [HttpPost("api/poses")]
    public async Task<ActionResult<CharacterPose>> CreatePose(CharacterPose pose)
    {
        _context.CharacterPoses.Add(pose);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPose), new { id = pose.Id }, pose);
    }

    [HttpPut("api/poses/{id}")]
    public async Task<IActionResult> UpdatePose(int id, CharacterPose pose)
    {
        if (id != pose.Id)
        {
            return BadRequest();
        }

        _context.Entry(pose).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PoseExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("api/poses/{id}")]
    public async Task<IActionResult> DeletePose(int id)
    {
        var pose = await _context.CharacterPoses.FindAsync(id);
        if (pose == null)
        {
            return NotFound();
        }

        _context.CharacterPoses.Remove(pose);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PoseExists(int id)
    {
        return _context.CharacterPoses.Any(e => e.Id == id);
    }
}