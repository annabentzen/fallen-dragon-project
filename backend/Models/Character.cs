using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DragonGame.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Hair { get; set; } = "hair1.png";
        public string Face { get; set; } = "face1.png";
        public string Clothing { get; set; } = "clothing1.png";
        
        [Display(Name = "Pose")]
        public int? PoseId { get; set; } // Foreign key
        public CharacterPose? Pose { get; set; } // Navigation property

    }
}
