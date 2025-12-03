using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonGame.Models;

public class Choice
{
    [Key]
    public int ChoiceId { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    [ForeignKey(nameof(Act))]
    public int ActId { get; set; }

    public Act Act { get; set; } = null!;

    /// <summary>
    /// Points to the ActNumber (not ActId) of the next act in the story branch.
    /// </summary>
    public int NextActNumber { get; set; }
}