using System.ComponentModel.DataAnnotations;

namespace DragonGame.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();
}