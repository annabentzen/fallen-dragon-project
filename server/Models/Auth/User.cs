using System.ComponentModel.DataAnnotations;

namespace DragonGame.Models
{
    /// <summary>
    /// Represents a registered user who can play the game
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// Username for login (must be unique)
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50)]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// BCrypt hashed password
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// When the user account was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

         // Navigation property for sessions
        public ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();
    }
}