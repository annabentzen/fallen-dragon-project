using DragonGame.Dtos;
using DragonGame.Dtos.ChoiceHistory;
using DragonGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonGame.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChoiceHistoryController : ControllerBase
{
    private readonly IChoiceHistoryService _service;
    private readonly ILogger<ChoiceHistoryController> _logger;

    public ChoiceHistoryController(IChoiceHistoryService service, ILogger<ChoiceHistoryController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Returns all choices made during a game session.
    /// </summary>
    [HttpGet("session/{sessionId}")]
    public async Task<ActionResult<IEnumerable<ChoiceHistoryDto>>> GetBySession(int sessionId)
    {
        var history = await _service.GetChoicesBySessionIdAsync(sessionId);

        var dtos = history.Select(h => new ChoiceHistoryDto
        {
            Id = h.Id,
            ActNumber = h.ActNumber,
            ChoiceId = h.ChoiceId,
            ChoiceText = h.Choice.Text,
            MadeAt = h.MadeAt
        });

        _logger.LogInformation(
            "Retrieved {Count} choices for session {SessionId}", 
            dtos.Count(), 
            sessionId);

        return Ok(dtos);
    }
}