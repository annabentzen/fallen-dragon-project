using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonGame.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string? Hair { get; set; } = "hair1.png";
        public string? Face { get; set; } = "face1.png";
        public string? Outfit { get; set; } = "clothing1.png";

        [Display(Name = "Pose")]
        [ForeignKey("Pose")]
        public int? PoseId { get; set; } // Foreign key
        public CharacterPose? Pose { get; set; } // Navigation property

    }
}
