using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragonGame.Models;

namespace YourNamespace.Models
{
    public class ChoiceHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlayerSessionId { get; set; }

        [ForeignKey(nameof(PlayerSessionId))]
        public PlayerSession PlayerSession { get; set; } = null!;

        [Required]
        public int ActNumber { get; set; }

        [Required]
        public int ChoiceId { get; set; }

        [ForeignKey(nameof(ChoiceId))]
        public Choice Choice { get; set; } = null!;

        [Required]
        public DateTime MadeAt { get; set; } = DateTime.UtcNow;
    }
}
