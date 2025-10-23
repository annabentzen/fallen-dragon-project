using Microsoft.AspNetCore.Mvc;
using DragonGame.Models;
using DragonGame.Repositories;
using DragonGame.Data;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StoryController : ControllerBase
    {
        private readonly IStoryRepository _repository;
        public StoryController(IStoryRepository repository) => _repository = repository;

        private readonly AppDbContext _context;

        public StoryController(AppDbContext context)
        {
            _context = context;
        }


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



        [HttpGet("{storyId}/acts/{actId}")]
        public async Task<IActionResult> GetAct(int storyId, int actId)
        {
            var act = await _context.Acts
                                    .Include(a => a.Choices)
                                    .FirstOrDefaultAsync(a => a.Id == actId && a.Id == storyId);

            if (act == null) return NotFound();

            return Ok(act);
        }





    }
}