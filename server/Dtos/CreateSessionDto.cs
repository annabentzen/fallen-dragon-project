using DragonGame.Models;

namespace DragonGame.Dtos
{
    public class CreateSessionDto
    {
        public int StoryId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
         public Character Character { get; set; } = new Character();
    }
}
