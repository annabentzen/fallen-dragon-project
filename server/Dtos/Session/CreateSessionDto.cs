using System.ComponentModel.DataAnnotations;

namespace DragonGame.Dtos;

public class CreateSessionDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Invalid story ID")]
    public int StoryId { get; set; }

    [Required(ErrorMessage = "Character name is required")]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Character name must be 1-30 characters")]
    public string CharacterName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Character is required")]
    public CharacterDto Character { get; set; } = new();
}
