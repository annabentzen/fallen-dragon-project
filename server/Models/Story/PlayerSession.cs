using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DragonGame.Models
{
    public class PlayerSession
    {
        [Key]
        public int SessionId { get; set; } // Session ID

        [Required]
        public string CharacterName { get; set; } = string.Empty;

        [Required]
        public string CharacterDesignJson { get; set; } = "{}"; // store design as JSON

        [NotMapped]
        public CharacterDesign CharacterDesign
        {
            get => string.IsNullOrEmpty(CharacterDesignJson)
                ? new CharacterDesign()
                : JsonSerializer.Deserialize<CharacterDesign>(CharacterDesignJson) ?? new CharacterDesign();
            set => CharacterDesignJson = JsonSerializer.Serialize(value);
        }

        [Required]
        public int StoryId { get; set; }

        [Required]
        public int CurrentActNumber { get; set; } = 1; // start at act 1

        [Required]
        public bool IsCompleted { get; set; } = false;
    }

    public class CharacterDesign
    {
        public string? Hair { get; set; }
        public string? Face { get; set; }
        public string? Outfit { get; set; }
        public int? PoseId { get; set; }
    }
}

