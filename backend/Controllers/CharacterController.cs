using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateAsync(Character character)
        {
            if (!ModelState.IsValid)
                return View(character);

            try
            {
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

        // GET: Character/Result/{id}
        // shows the created character
        public async Task<IActionResult> Result(int id)
        {
            var character = await _repository.GetByIdAsync(id);
            if (character == null) return RedirectToAction(nameof(Create));
            return View(character);
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