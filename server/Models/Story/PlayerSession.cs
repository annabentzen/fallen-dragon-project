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

        [Required]
        public int CharacterId { get; set; } // FK to Character

        [ForeignKey(nameof(CharacterId))]
        public Character Character { get; set; } = null!;

        [Required]
        public int StoryId { get; set; }

        public Story Story { get; set; } = null!;
        public int CurrentActNumber { get; set; }

        [ForeignKey("CurrentActNumber")]
        public Act CurrentAct { get; set; } = null!;

        [Required]
        public bool IsCompleted { get; set; } = false;

        // Navigation property for ChoiceHistory
        public ICollection<ChoiceHistory> Choices { get; set; } = new List<ChoiceHistory>();
    }
}


