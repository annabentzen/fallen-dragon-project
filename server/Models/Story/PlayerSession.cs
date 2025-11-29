using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonGame.Models
{
    public class PlayerSession
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        public string CharacterName { get; set; } = string.Empty;

        // Add to existing PlayerSession class:

        // Foreign key to User
        public int UserId { get; set; }

        // Navigation property
        public User? User { get; set; }

        [Required]
        public int CharacterId { get; set; } // FK to Character

        [ForeignKey(nameof(CharacterId))]
        public Character Character { get; set; } = null!;

        [Required]
        public int StoryId { get; set; }

        public Story Story { get; set; } = null!;

        [Required]
        public int CurrentActNumber { get; set; } = 1;
        
        [ForeignKey("CurrentActNumber")]
        public Act? CurrentAct { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        // Navigation property for ChoiceHistory
        public ICollection<ChoiceHistory> Choices { get; set; } = new List<ChoiceHistory>();
    }
}


