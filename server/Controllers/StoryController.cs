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

    [HttpPost("start")]
    public async Task<IActionResult> StartStory()
    {
        // Get the first story in DB
        var story = await _context.Stories.FirstOrDefaultAsync();
        if (story == null)
            return NotFound("No story found.");

        // Create a new session tied to that story
        var session = new PlayerSession
        {
            StoryId = story.Id,
            CurrentActNumber = 1
        };

        _context.PlayerSessions.Add(session);
        await _context.SaveChangesAsync();

        // Return the new sessionId to the frontend
        return Ok(new { sessionId = session.SessionId });
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


    [HttpGet("{sessionId}/current")]
    public async Task<IActionResult> GetCurrentActForSession(Guid sessionId)
    {
        var session = await _context.PlayerSessions.FirstOrDefaultAsync(s => s.SessionId == sessionId);
        if (session == null)
            return NotFound("Session not found.");

        var act = await _context.Acts
            .Include(a => a.Choices)
            .FirstOrDefaultAsync(a => a.ActNumber == session.CurrentActNumber && a.StoryId == session.StoryId);

        if (act == null)
            return NotFound("Act not found for this session.");

        return Ok(act);
    }

    [HttpPost("{sessionId}/progress/{nextActNumber}")]
    public async Task<IActionResult> UpdateSessionProgress(Guid sessionId, int nextActNumber)
    {
        var session = await _context.PlayerSessions.FirstOrDefaultAsync(s => s.SessionId == sessionId);
        if (session == null)
            return NotFound("Session not found.");

        session.CurrentActNumber = nextActNumber;
        session.LastUpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Progress updated" });
    }




}



