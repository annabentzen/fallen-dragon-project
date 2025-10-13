using System.Collections.Generic;

namespace DragonGame.Models
{
    public class CharacterPose
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public List<Character> Characters { get; set; } = new();
    }
}

