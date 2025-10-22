using Microsoft.AspNetCore.Mvc;
using DragonGame.Models.Story;
using DragonGame.Repositories;
 

[ApiController]
[Route("api/[controller]")]
public class StoryController : ControllerBase
{
    private readonly IStoryRepository _repository;
    public StoryController(IStoryRepository repository) => _repository = repository;

    [HttpGet]
    public async Task<IActionResult> GetStories() => Ok(await _repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStory(int id)
    {
        var story = await _repository.GetByIdAsync(id);
        return story == null ? NotFound() : Ok(story);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStory([FromBody] Story story)
    {
        var created = await _repository.CreateAsync(story);
        return CreatedAtAction(nameof(GetStory), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStory(int id, [FromBody] Story story)
    {
        if (id != story.Id) return BadRequest();
        await _repository.UpdateAsync(story);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStory(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
