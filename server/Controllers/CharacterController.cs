using Microsoft.AspNetCore.Mvc;
using DragonGame.Services;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;
using DragonGame.Data;
using server.Services.Interfaces;
using DragonGame.Dtos;

namespace DragonGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        private readonly AppDbContext _context;

        public CharacterController(ICharacterService characterService, AppDbContext context)
        {
            _characterService = characterService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var chars = await _characterService.GetAllAsync();
            return Ok(chars);
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

        // NEW: Update character through session
        [HttpPut("session/{sessionId}")]
        public async Task<IActionResult> UpdateCharacterForSession(int sessionId, [FromBody] UpdateCharacterDto dto)
        {
            try
            {
                var session = await _context.PlayerSessions
                    .Include(s => s.Character)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null || session.Character == null)
                    return NotFound("Session or character not found");

                // Update only the appearance properties
                session.Character.Head = dto.Head;
                session.Character.Body = dto.Body;
                session.Character.PoseId = dto.PoseId;

                await _context.SaveChangesAsync();
                return Ok(session.Character);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CharacterController][UpdateForSession] Error: {ex.Message}");
                return StatusCode(500, $"Error updating character: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _characterService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}