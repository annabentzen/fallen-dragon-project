using System.ComponentModel.DataAnnotations;

namespace DragonGame.Models;

public class Story
{
    [Key]
    public int StoryId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    public List<Act> Acts { get; set; } = new();

    public ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();
}