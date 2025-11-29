using DragonGame.Dtos;
using DragonGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChoiceHistoryController : ControllerBase
    {
        private readonly IChoiceHistoryService _service;

        public ChoiceHistoryController(IChoiceHistoryService service)
        {
            _service = service;
        }

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

            return Ok(dtos);
        }
    }
}