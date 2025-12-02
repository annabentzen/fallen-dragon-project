using System.ComponentModel.DataAnnotations;

namespace DragonGame.Models;

public class CharacterPose
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Restricts pose to specific character types ("knight", "mage1", "mage2", "rogue").
    /// </summary>
    public string? CharacterType { get; set; }

    public ICollection<Character> Characters { get; set; } = new List<Character>();
}