using Microsoft.AspNetCore.Mvc;
using DragonGame.Services;

namespace DragonGame.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ICharacterAssetsService _assetsService;

        public ApiController(ICharacterAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        [HttpGet("hair")]
        public IActionResult GetHairOptions()
        {
            var hairOptions = _assetsService.GetHairOptions();
            return Ok(hairOptions);
        }

        [HttpGet("faces")]
        public IActionResult GetFaceOptions()
        {
            var faceOptions = _assetsService.GetFaceOptions();
            return Ok(faceOptions);
        }

        [HttpGet("clothes")]
        public IActionResult GetClothingOptions()
        {
            var clothesOptions = _assetsService.GetClothingOptions();
            return Ok(clothesOptions);
        }
    }
}
