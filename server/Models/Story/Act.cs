using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonGame.Models;

public class Act
{
    [Key]
    public int ActId { get; set; }

    /// <summary>
    /// Story progression identifier. Used as navigation key instead of ActId
    /// to support branching narratives (e.g., Act 1 → Act 11 or Act 12).
    /// </summary>
    public int ActNumber { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    public bool IsEnding { get; set; }

    [ForeignKey(nameof(Story))]
    public int StoryId { get; set; }

    public Story Story { get; set; } = null!;

    public List<Choice> Choices { get; set; } = new();
}