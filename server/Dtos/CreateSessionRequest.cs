using DragonGame.Models;

namespace DragonGame.Models.DTOs
{
    public class CreateSessionDto
    {
        public string CharacterName { get; set; } = string.Empty;
        public Character Character { get; set; } = new Character();
        public int StoryId { get; set; }
    }
}
