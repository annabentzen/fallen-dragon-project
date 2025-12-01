using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonGame.Models
{
    public class Character
    {
        [Key]
        public int Id { get; set; }

        public string? Head { get; set; } = "knight-head.png";
        public string? Body { get; set; } = "knight-body.png";

        [Display(Name = "Pose")]
        [ForeignKey("Pose")]
        public int? PoseId { get; set; }

        public CharacterPose? Pose { get; set; }
        public ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();
    }
}
