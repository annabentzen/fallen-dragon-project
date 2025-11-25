using System.ComponentModel.DataAnnotations;

namespace DragonGame.Models
{
        public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property: One user can have many game sessions
        public ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();
    }
}