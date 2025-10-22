using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace DragonGame.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        [HttpGet("hair")]
        public IActionResult GetHairOptions()
        {
            var hairOptions = Directory.GetFiles("wwwroot/images/hair")
                .Select(Path.GetFileName)
                .ToList();

            return Ok(hairOptions);
        }

        [HttpGet("faces")]
        public IActionResult GetFaceOptions()
        {
            var faceOptions = Directory.GetFiles("wwwroot/images/faces")
                .Select(Path.GetFileName)
                .ToList();

            return Ok(faceOptions);
        }

        [HttpGet("clothes")]
        public IActionResult GetClothingOptions()
        {
            var clothesOptions = Directory.GetFiles("wwwroot/images/clothes")
                .Select(Path.GetFileName)
                .ToList();

            return Ok(clothesOptions);
        }
    }
}
