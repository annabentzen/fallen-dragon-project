namespace DragonGame.Models
{
    public class CharacterPose
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? CharacterType { get; set; } // "knight", "mage", or "rogue"
    
        public ICollection<Character> Characters { get; set; } = new List<Character>();

    }
}

