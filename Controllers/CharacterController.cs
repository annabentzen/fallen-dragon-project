using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyMvcApp.Data;
using MyMvcApp.Models;

namespace MyMvcApp.Controllers
{
    public class CharacterController : Controller
    {
        private readonly MyMvcAppDbContext _context;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(MyMvcAppDbContext context, ILogger<CharacterController> logger)
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
        public async Task<IActionResult> CreateAsync(Character character)
        {
            if (!ModelState.IsValid)
                return View(character);

            try
            {
                await _context.Characters.AddAsync(character);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Result), new { id = character.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving character");
                ModelState.AddModelError("", "Error saving character, try again.");
                return View(character);
            }
        }

        // GET: Character/Result/5
        public async Task<IActionResult> Result(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null) return RedirectToAction(nameof(Create));
            return View(character);
        }

        // POST: Character/Delete/5
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
