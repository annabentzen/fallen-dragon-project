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
        try
        {
            var characters = await _characterService.GetAllAsync();
            return Ok(characters);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CharacterController][GetAll] Error: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var character = await _characterService.GetByIdAsync(id);
            if (character == null) return NotFound();
            return Ok(character);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CharacterController][GetById] Error for id={id}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(Character character)
    {
        try
        {
            var created = await _characterService.CreateAsync(character);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CharacterController][Create] Error: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Character character)
    {
        try
        {
            var updated = await _characterService.UpdateAsync(id, character);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CharacterController][Update] Error for id={id}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _characterService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CharacterController][Delete] Error for id={id}: {ex}");
            return StatusCode(500, ex.Message);
        }
    }
}



}
