using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DragonGame.Data;
using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Services;

namespace DragonGame.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _characterService;
    private readonly AppDbContext _context;
    private readonly ILogger<CharacterController> _logger;

    public CharacterController(
        ICharacterService characterService, 
        AppDbContext context,
        ILogger<CharacterController> logger)
    {
        _characterService = characterService;
        _context = context;
        _logger = logger;
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
        
        if (character == null)
        {
            _logger.LogWarning("Character not found: {CharacterId}", id);
            return NotFound(new { message = "Character not found" });
        }
        
        return Ok(character);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Character character)
    {
        var created = await _characterService.CreateAsync(character);
        
        _logger.LogInformation("Character created: {CharacterId}", created.Id);
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Character character)
    {
        var updated = await _characterService.UpdateAsync(id, character);
        
        if (updated == null)
        {
            _logger.LogWarning("Update failed - character not found: {CharacterId}", id);
            return NotFound(new { message = "Character not found" });
        }

        _logger.LogInformation("Character updated: {CharacterId}", id);
        return Ok(updated);
    }

    /// <summary>
    /// Updates character appearance during gameplay via session ID.
    /// </summary>
    [HttpPut("session/{sessionId}")]
    public async Task<IActionResult> UpdateCharacterForSession(int sessionId, [FromBody] UpdateCharacterDto dto)
    {
        var session = await _context.PlayerSessions
            .Include(s => s.Character)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session?.Character == null)
        {
            _logger.LogWarning("Update failed - session or character not found: {SessionId}", sessionId);
            return NotFound(new { message = "Session or character not found" });
        }

        session.Character.Head = dto.Head;
        session.Character.Body = dto.Body;
        session.Character.PoseId = dto.PoseId;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Character appearance updated for session {SessionId}: Head={Head}, Body={Body}, PoseId={PoseId}",
            sessionId, dto.Head, dto.Body, dto.PoseId);

        return Ok(session.Character);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _characterService.DeleteAsync(id);
        
        if (!deleted)
        {
            _logger.LogWarning("Delete failed - character not found: {CharacterId}", id);
            return NotFound(new { message = "Character not found" });
        }

        _logger.LogInformation("Character deleted: {CharacterId}", id);
        return NoContent();
    }
}