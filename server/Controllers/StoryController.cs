[Route("api/[controller]")]
[ApiController]
public class StoryController : ControllerBase
{
    private readonly DragonGameDbContext _context;

    public StoryController(DragonGameDbContext context)
    {
        _context = context;
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

