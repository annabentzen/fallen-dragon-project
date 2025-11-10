using DragonGame.Models;

namespace DragonGame.Dtos
{
    public class CreateSessionDto
    {
        public int StoryId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public CharacterDesign CharacterDesign { get; set; } = new CharacterDesign();
    }
}
