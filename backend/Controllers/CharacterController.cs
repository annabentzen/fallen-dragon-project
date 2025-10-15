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
        // repository for character poses
        private readonly ICharacterPoseRepository _poseRepository;
        // logger for error logging
        private readonly ILogger<CharacterController> _logger;

        // constructor with dependency injection
        public CharacterController(ICharacterRepository repository, ICharacterPoseRepository poseRepository, ILogger<CharacterController> logger)
        {
            _repository = repository;
            _poseRepository = poseRepository;
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

            // if poseid not set, set default pose
            character.PoseId = 1;

            if (!ModelState.IsValid)
            {
                return View(character);
            }

            try
            {
                await _repository.AddAsync(character); // save character to database
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


        // GET: Character/Result/{id}
        // shows the created character and allows selecting/updating pose
        public async Task<IActionResult> Result(int id)
        {
            // Get character via repository
            var character = await _repository.GetByIdAsync(id);

            if (character == null)
                return RedirectToAction(nameof(Create));

            // Populate dropdown for poses
            var poses = await _poseRepository.GetAllAsync();
            ViewBag.PoseOptions = poses.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            })
            .ToList();

            return View(character);
        }

        // POST: Character/UpdateCharacterPose
        // updates the pose of an existing character
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCharacterPose(int id, int? selectedPoseId)
        {
            var character = await _repository.GetByIdAsync(id);
            if (character == null)
                return NotFound();

            character.PoseId = selectedPoseId;
            await _repository.UpdateAsync(character); // update via repository

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