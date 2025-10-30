using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonGame.Models
{
    public class PlayerSession
    {
        [Key]
        public Guid SessionId { get; set; } = Guid.NewGuid(); // unique ID

        public int CurrentActNumber { get; set; } = 1; // default: Act 1

        // Optional: store metadata
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }

        // Foreign key to Story (optional if you support multiple stories)
        public int StoryId { get; set; }

        [ForeignKey("StoryId")]
        public Story Story { get; set; }
    }
}
