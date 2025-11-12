using DragonGame.Models;
using DragonGame.Repositories;
using server.Services.Interfaces;

namespace server.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;

        public CharacterService(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }

        // Get all characters with their pose
        public async Task<IEnumerable<Character>> GetAllAsync()
        {
            return await _characterRepository.GetAllAsync();
        }

        // Get character by ID with pose
        public async Task<Character?> GetByIdAsync(int id)
        {
            return await _characterRepository.GetByIdAsync(id);
        }

        // Create a new character
        public async Task<Character> CreateAsync(Character character)
        {
            await _characterRepository.AddAsync(character);
            return character;
        }

        // Update an existing character
        public async Task<Character?> UpdateAsync(int id, Character updatedCharacter)
        {
            var existing = await _characterRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Hair = updatedCharacter.Hair;
            existing.Face = updatedCharacter.Face;
            existing.Outfit = updatedCharacter.Outfit;
            existing.PoseId = updatedCharacter.PoseId;

            await _characterRepository.UpdateAsync(existing);

            return existing;
        }

        // Delete a character by ID
        public async Task<bool> DeleteAsync(int id)
        {
            var character = await _characterRepository.GetByIdAsync(id);
            if (character == null) return false;

            await _characterRepository.DeleteAsync(character);
            return true;
        }
    }
}

