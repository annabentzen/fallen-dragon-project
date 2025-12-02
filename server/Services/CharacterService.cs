using DragonGame.Models;
using DragonGame.Repositories;

namespace DragonGame.Services;

public class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _characterRepository;

    public CharacterService(ICharacterRepository characterRepository)
    {
        _characterRepository = characterRepository;
    }

    public async Task<IEnumerable<Character>> GetAllAsync()
    {
        return await _characterRepository.GetAllAsync();
    }

    public async Task<Character?> GetByIdAsync(int id)
    {
        return await _characterRepository.GetByIdAsync(id);
    }

    public async Task<Character> CreateAsync(Character character)
    {
        await _characterRepository.AddAsync(character);
        return character;
    }

    public async Task<Character?> UpdateAsync(int id, Character character)
    {
        var existing = await _characterRepository.GetByIdAsync(id);
        if (existing == null) return null;

        existing.Head = character.Head;
        existing.Body = character.Body;
        existing.PoseId = character.PoseId;

        await _characterRepository.UpdateAsync(existing);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var character = await _characterRepository.GetByIdAsync(id);
        if (character == null) return false;

        await _characterRepository.DeleteAsync(id);
        return true;
    }
}