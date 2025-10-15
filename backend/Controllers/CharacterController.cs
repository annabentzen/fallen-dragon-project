using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DragonGame.Models;
using DragonGame.Repositories;

namespace DragonGame.Controllers
{
    public class CharacterController : Controller
    {

        // repository for dataoperations
        private readonly ICharacterRepository _repository;
        // logger for error logging
        private readonly ILogger<CharacterController> _logger;

        // constructor with dependency injection
        public CharacterController(ICharacterRepository repository, ILogger<CharacterController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: Character/Create
        // opens character creation view
        public IActionResult Create()
        {
            return View(new Character());
        }

        // POST: Character/Create
        // receives character data from form and saves a new character
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Character character)
        {
            _logger.LogInformation("Create called. ModelState.IsValid={IsValid}", ModelState.IsValid);

             // Assign a default pose (e.g., first pose in DB)
            character.PoseId = _context.CharacterPoses.FirstOrDefault()?.Id;

            if (!ModelState.IsValid)
            {
                return View(character);
            }

            try
            {
                await _context.Characters.AddAsync(character);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saved Character with Id: {Id}", character.Id);

                await _repository.AddAsync(character); // save character to database
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

        // GET: Character/Result/{id}
        // shows the created character
        public async Task<IActionResult> Result(int id)
        {
            var character = await _repository.GetByIdAsync(id);
            if (character == null) return RedirectToAction(nameof(Create));
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
        // deletes the character and redirects to create new character
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Create));
        }
    }
}