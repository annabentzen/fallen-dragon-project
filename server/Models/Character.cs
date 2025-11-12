using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonGame.Models
{
    /// <summary>
    /// Represents a character with visual attributes and pose.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// Unique identifier for the character (entity)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Filename for the hair style of the character.
        /// Must follow the predefined options
        /// </summary>
        [Required(ErrorMessage = "Hair style is required")]
        [StringLength(50, ErrorMessage = "Hair filename cannot be over 50 characters")]
        [RegularExpression(@"^hair[1-3]\.png$", ErrorMessage = "Hair must be in the format 'hairX.png' where X is 1, 2, or 3.")]
        public string? Hair { get; set; } = "hair1.png";

        /// <summary>
        /// Filename for the selected face of the character.
        /// Must follow the predefined options
        /// </summary>
        [Required(ErrorMessage = "Face style is required")]
        [StringLength(50, ErrorMessage = "Face filename cannot be over 50 characters")]
        [RegularExpression(@"^face[1-3]\.png$", ErrorMessage = "Face must be in the format 'faceX.png' where X is 1, 2, or 3.")]
        public string? Face { get; set; } = "face1.png";

        /// <summary>
        /// Filename for the outfit style of the character.
        /// Must follow the predefined options
        /// </summary>
        [Required(ErrorMessage = "Outfit style is required")]
        [StringLength(50, ErrorMessage = "Outfit filename cannot be over 50 characters")]
        [RegularExpression(@"^clothing[1-3]\.png$", ErrorMessage = "Outfit must be in the format 'clothingX.png' where X is 1, 2, or 3.")]
        public string? Outfit { get; set; } = "clothing1.png";


        /// <summary>
        /// Navigation property for the character's pose.
        /// Links to CharacterPose entity.
        /// </summary>
        [Display(Name = "Pose")]
        [ForeignKey("Pose")]
        public int? PoseId { get; set; } // Foreign key
        public CharacterPose? Pose { get; set; } // Navigation property

    }
}
