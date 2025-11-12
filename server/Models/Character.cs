using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonGame.Models
{
    public class Character
    {
        [Key]
        public int Id { get; set; }

        public string? Hair { get; set; } = "hair1.png";
        public string? Face { get; set; } = "face1.png";
        public string? Outfit { get; set; } = "clothing1.png";

        [Display(Name = "Pose")]
        [ForeignKey("Pose")]
        public int? PoseId { get; set; }

        public CharacterPose? Pose { get; set; }

        // Navigation property for sessions
        public ICollection<PlayerSession> Sessions { get; set; } = new List<PlayerSession>();
    }
}
