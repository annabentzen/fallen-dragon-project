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
        public async Task<IActionResult> Create()
        {
            // Create an empty character so the view can reference its properties
            var model = new Character
            {
                Hair = "hair1.png",
                Face = "face1.png",
                Clothing = "clothing1.png",
                PoseId = null
            };

            // Optionally load pose options into ViewBag
            var poses = await _poseRepository.GetAllPosesAsync();
            ViewBag.PoseOptions = new SelectList(poses, "Id", "Name");

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Character character)
        {
            if (ModelState.IsValid)
            {
                await _characterRepository.AddAsync(character);
                return RedirectToAction("Result", new { id = character.Id });
            }

            var poses = await _poseRepository.GetAllPosesAsync();
            ViewBag.PoseOptions = new SelectList(poses, "Id", "Name");
            return View(character);
        }

        // RESULT VIEW
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
        public async Task<IActionResult> UpdateCharacterPose(int id, int? poseId)
        {
            var character = await _characterRepository.GetByIdAsync(id);
            if (character == null)
                return NotFound();

            character.PoseId = poseId;
            await _characterRepository.UpdateAsync(character);

            return RedirectToAction("Result", new { id });
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
