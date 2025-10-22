using DragonGame.Models;
using DragonGame.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace DragonGame.Controllers
{
    public class CharacterController : Controller
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ICharacterPoseRepository _poseRepository;

        public CharacterController(ICharacterRepository characterRepository, ICharacterPoseRepository poseRepository)
        {
            _characterRepository = characterRepository;
            _poseRepository = poseRepository;
        }

        // CREATE VIEW
        // GET: Character/Create
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
                Clothing = "clothing1.png",
                PoseId = null
            };

            return View(character);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Character character)
        {
            if (ModelState.IsValid)
            {
                // Save character to db
                await _characterRepository.AddAsync(character);
                //redirect to result view by Id
                return RedirectToAction("Result", new { id = character.Id });
            }

            var poses = await _poseRepository.GetAllPosesAsync();
            ViewBag.PoseOptions = new SelectList(poses, "Id", "Name");
            return View(character);
        }

        // RESULT VIEW
        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            var character = await _characterRepository.GetByIdAsync(id);
            if (character == null)
                return NotFound();

            var poses = await _poseRepository.GetAllPosesAsync();
            ViewBag.PoseOptions = new SelectList(poses, "Id", "Name", character.PoseId);

            return View(character);
        }


        // UPDATE CHARACTER POSE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCharacterPose(int id, int? poseId, string Hair, string Face, string Clothing)
        {
            var character = await _characterRepository.GetByIdAsync(id);
            if (character == null)
                return NotFound();

            // Update pose
            character.PoseId = poseId;
            if (poseId.HasValue)
                character.Pose = await _poseRepository.GetByIdAsync(poseId.Value);

            // Update hair, face, clothing from hidden inputs
            character.Hair = Hair ?? character.Hair;
            character.Face = Face ?? character.Face;
            character.Clothing = Clothing ?? character.Clothing;

            await _characterRepository.UpdateAsync(character); // Save to db

            return RedirectToAction(nameof(Result), new { id = character.Id });
        }



        // DELETE CHARACTER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _characterRepository.DeleteAsync(id);
            return RedirectToAction("Create");
        }
    }
}
