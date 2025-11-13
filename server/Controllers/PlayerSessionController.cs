using DragonGame.Dtos;
using DragonGame.Models;
using DragonGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragonGame.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PlayerSessionController : ControllerBase
    {
        private readonly PlayerSessionService _service;

        public PlayerSessionController(PlayerSessionService service)
        {
            _service = service;
        }

        // GET api/playersession/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerSessionDto>> GetSession(int id)
        {
            var dto = await _service.GetSessionDtoAsync(id);
            if (dto == null) return NotFound();

            return Ok(dto);
        }
    }
}