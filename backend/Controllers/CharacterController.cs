using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DragonGame.Data;
using DragonGame.Models;

namespace DragonGame.Controllers
{
    public class CharacterController : Controller
    {
        private readonly DragonGameDbContext _context;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(DragonGameDbContext context, ILogger<CharacterController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Character/Create
        public IActionResult Create()
        {
            return View(new Character());
        }

        // POST: Character/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Character character)
        {
            _logger.LogInformation("Create called. ModelState.IsValid={IsValid}", ModelState.IsValid);

             // If PoseId is null, assign default pose (for example, ID = 1)
            character.PoseId ??= 3;

            if (!ModelState.IsValid)
            {
                return View(character);
            }

            try
            {
                await _context.Characters.AddAsync(character);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saved Character with Id: {Id}", character.Id);

                return RedirectToAction(nameof(Result), new { id = character.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving character");
                ModelState.AddModelError("", "Error saving character, try again.");
                return View(character);
            }
        }



        /// GET: Character/Result/{id}
        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            var character = await _context.Characters
                .Include(c => c.Pose)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (character == null)
                return NotFound();

            // Populate dropdown
            ViewBag.PoseOptions = _context.CharacterPoses
                                        .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.Name
                                        }).ToList();

            return View(character);
        }

        // POST: Character/UpdateCharacterPose
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCharacterPose(int id, int? selectedPoseId)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
                return NotFound();

            character.PoseId = selectedPoseId;

            await _context.SaveChangesAsync();

            return RedirectToAction("Result", new { id = character.Id });
        }



        // POST: Character/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character != null)
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Create));
        }
    }
}