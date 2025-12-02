using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonGame.Models;

/// <summary>
/// Represents a single playthrough of a story by a user.
/// </summary>
public class PlayerSession
{
    [Key]
    public int SessionId { get; set; }

    [Required]
    [StringLength(30)]
    public string CharacterName { get; set; } = string.Empty;

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public User? User { get; set; }

    [ForeignKey(nameof(Character))]
    public int CharacterId { get; set; }

    public Character Character { get; set; } = null!;

    [ForeignKey(nameof(Story))]
    public int StoryId { get; set; }

    public Story Story { get; set; } = null!;

    /// <summary>
    /// References ActNumber (not ActId) to track position in branching narrative.
    /// </summary>
    public int CurrentActNumber { get; set; } = 1;

    public Act? CurrentAct { get; set; }

    public bool IsCompleted { get; set; }

    public ICollection<ChoiceHistory> Choices { get; set; } = new List<ChoiceHistory>();
}

