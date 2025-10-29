using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using DragonGame.Data; // For DragonGameDbContext
using DragonGame.Models; // For Story, Act, etc.
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class StoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public StoryController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetStory()
    {
        var story = await _context.Stories
                                  .Include(s => s.Acts)
                                  .ThenInclude(a => a.Choices)
                                  .FirstOrDefaultAsync();
        if (story == null) return NotFound();
        return Ok(story);
    }


    [HttpGet("act/{number}")]
    public IActionResult GetAct(int number)
    {
        var act = _context.Acts
            .Include(a => a.Choices)
            .FirstOrDefault(a => a.ActNumber == number);

        if (act == null)
            return NotFound();

        return Ok(act);
    }

    [HttpGet("choices/{actId}")]
    public IActionResult GetChoicesForAct(int actId)
    {
        var choices = _context.Choices
            .Where(c => c.ActId == actId)
            .ToList();

        return Ok(choices);
    }
}

