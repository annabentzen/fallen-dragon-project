using System.Collections.Generic;

namespace DragonGame.Models
{
    public class CharacterPose
    {
        public int Id { get; set; }
        public string Name { get; set; } = "pose1.png";
        public string Description { get; set; } = "A default pose";

        public List<Character> Characters { get; set; } = new();
    }
}

