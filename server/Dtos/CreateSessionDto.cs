using DragonGame.Models;
using System.ComponentModel.DataAnnotations;

namespace DragonGame.Dtos
{
    /// <summary>
    /// Data transfer object for creating a new player session.
    /// contains all data needed to start a story session.
    /// </summary>
    public class CreateSessionDto
    {
        /// <summary>
        /// The ID of the story to start.
        /// Must be a positive integer corresponding to an existing story in the db.
        /// </summary>
        [Required(ErrorMessage = "Story ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Story ID must be a positive integer.")]
        public int StoryId { get; set; }
        /// <summary>
        /// The hero name (character name) chosen by the player.
        /// Must be between 2-50 characters and contain only letters, numbers and spaces.
        /// </summary>
        [Required(ErrorMessage = "Character name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Character name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Character name can only contain letters, numbers, and spaces.")]
        public string CharacterName { get; set; } = string.Empty;

        /// <summary>
        /// the character design choices incl. hair, face, outfit, pose.
        /// serialized to JSON and stored in the db for retrieval during the story session.
        /// </summary>
        [Required(ErrorMessage = "Character design is required.")]
        public CharacterDesign CharacterDesign { get; set; } = new CharacterDesign();
    }
}
