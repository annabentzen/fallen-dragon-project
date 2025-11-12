using DragonGame.Models;
using Microsoft.AspNetCore.Mvc;
using server.Services.Interfaces;

namespace DragonGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var characters = await _characterService.GetAllAsync();
            return Ok(characters);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var character = await _characterService.GetByIdAsync(id);
            if (character == null) return NotFound();
            return Ok(character);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Character character)
        {
            var created = await _characterService.CreateAsync(character);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Character character)
        {
            var updated = await _characterService.UpdateAsync(id, character);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _characterService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }


}
