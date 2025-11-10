using DragonGame.Models;
using DragonGame.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace DragonGame.Controllers
{
    /// <summary>
    /// MVC Controller for managing character creation, updates, and deletion.
    /// Handles the character builder flow: create, customize, result, update pose.
    /// </summary>
    public class CharacterController : Controller
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ICharacterPoseRepository _poseRepository;

        /// <summary>
        /// Initializes a new instance of CharacterController with repositories.
        /// uses dependency injection to implement in repositories
        /// </summary>
        /// <param name="characterRepository">Repository for character data operations</param>
        /// <param name="poseRepository">Repository for character pose data operations</param>

        public CharacterController(ICharacterRepository characterRepository, ICharacterPoseRepository poseRepository)
        {
            _characterRepository = characterRepository;
            _poseRepository = poseRepository;
        }

        /// <summary>
        /// Displays the character creation view.
        /// GET: Character/Create
        /// </summary>
        /// <returns>View with a new character model and available pose options</returns>

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Get all poses from repository
            var poses = await _poseRepository.GetAllPosesAsync();

            // Pass to ViewBag for dropdown
            ViewBag.PoseOptions = new SelectList(poses, "Id", "Name");

            // Initialize a new character
            var character = new Character
            {
                Hair = "hair1.png",
                Face = "face1.png",
                Outfit = "clothing1.png",
                PoseId = null // pose is optional
            };

            return View(character);
        }

        /// <summary>
        /// Handles character creation form submission
        /// Validates and saves the new character to the database
        /// POST: Character/Create 
        /// </summary>
        /// <param name="character">Character object from form data</param>
        /// <returns>Redirects to the result view if successful</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent CSRF attacks
        public async Task<IActionResult> Create(Character character)
        {
            // server-side validation: check model state
            if (ModelState.IsValid)
            {
                // Save character to db
                await _characterRepository.AddAsync(character);
                //redirect to result view by Id
                return RedirectToAction("Result", new { id = character.Id });
            }

            // If validation fails, reload poses and return form with errors
            var poses = await _poseRepository.GetAllPosesAsync();
            ViewBag.PoseOptions = new SelectList(poses, "Id", "Name");
            return View(character);
        }

        /// <summary>
        /// Displays the result view for a created character.
        /// GET: Character/Result/{id}
        /// </summary>
        /// <param name="id">The ID of the character</param>
        /// <returns>View with character data and pose options, or NotFound if character doesn't exist</returns>
        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            // Fetch character by id from db
            var character = await _characterRepository.GetByIdAsync(id);
            if (character == null)
                return NotFound();

            // Load all poses for potential updates
            var poses = await _poseRepository.GetAllPosesAsync();
            ViewBag.PoseOptions = new SelectList(poses, "Id", "Name", character.PoseId);

            return View(character);
        }

        /// <summary>
        /// Updates the pose and other attributes of an existing character.
        /// POST: Character/UpdateCharacterPose
        /// </summary>
        /// <param name="id">The ID of the character</param>
        /// <param name="poseId">The new pose ID</param>
        /// <param name="Hair">Hair filename</param>
        /// <param name="Face">Face filename</param>
        /// <param name="Outfit">Outfit filename</param>
        /// <returns>Redirects to the result view if successful</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents CSRF attacks
        public async Task<IActionResult> UpdateCharacterPose(int id, int? poseId, string Hair, string Face, string Outfit)
        {
            // Fetch character from db
            var character = await _characterRepository.GetByIdAsync(id);
            // Return 404 if not found
            if (character == null)
                return NotFound();

            // Update pose ID
            character.PoseId = poseId;

            // load full pose object if poseId provided
            if (poseId.HasValue)
                character.Pose = await _poseRepository.GetByIdAsync(poseId.Value);

            // Update hair, face, outfit from inputs
            character.Hair = Hair ?? character.Hair;
            character.Face = Face ?? character.Face;
            character.Outfit = Outfit ?? character.Outfit;

            await _characterRepository.UpdateAsync(character); // Save to db

            // Redirect back to result view
            return RedirectToAction(nameof(Result), new { id = character.Id });
        }

        /// <summary>
        /// Deletes a character by ID.
        /// POST: Character/Delete/{id}
        /// </summary>
        /// <param name="id">The ID of the character to delete</param>
        /// <returns>Redirects to the create view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents CSRF attacks
        public async Task<IActionResult> Delete(int id)
        {
            await _characterRepository.DeleteAsync(id);
            // Redirect to create view after deletion
            return RedirectToAction("Create");
        }
    }
}
