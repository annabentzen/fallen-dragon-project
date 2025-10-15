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
            ViewBag.PoseOptions = new SelectList(_context.CharacterPoses, "Id", "Name");
            return View(new Character());
        }

        // POST: Character/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Character character)
        {
            _logger.LogInformation("CreateAsync called. ModelState.IsValid={IsValid}", ModelState.IsValid);
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogWarning("ModelState error: {Error}", error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState invalid: {@ModelState}", ModelState);
                ViewBag.PoseOptions = new SelectList(_context.CharacterPoses, "Id", "Name");
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
                ViewBag.PoseOptions = new SelectList(_context.CharacterPoses, "Id", "Name");
                return View(character);
            }
        }


        // GET: Character/Result/{id}
        [HttpGet]
        public async Task<IActionResult> Result(int id) // Add 'id' parameter to fetch specific character
        {
            // Fetch the character by ID, INCLUDING its associated Pose
            var character = await _context.Characters
                                        .Include(c => c.Pose) // Include the navigation property
                                        .FirstOrDefaultAsync(c => c.Id == id);

            if (character == null)
            {
                return NotFound(); // Character not found
            }

            return View(character);
        }


        // POST: Character/Update/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCharacterPose(int id, int? selectedPoseId) // Match nullable PoseId
        {
            // 1. Find the character to update
            var characterToUpdate = await _context.Characters.FindAsync(id);

            if (characterToUpdate == null)
            {
                return NotFound(); // Character with given ID not found
            }

            // 2. Update the PoseId
            characterToUpdate.PoseId = selectedPoseId;

            try
            {
                // 3. Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle potential concurrency conflicts
                if (!_context.Characters.Any(e => e.Id == id))
                {
                    return NotFound(); // Character was deleted by another process
                }
                else
                {
                    throw; // Re-throw other concurrency exceptions
                }
            }

            // 4. Redirect to the Result page for the updated character
            return RedirectToAction("Result", new { id = characterToUpdate.Id });
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