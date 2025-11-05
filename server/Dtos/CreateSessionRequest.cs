namespace DragonGame.Dtos
{
    public class CreateSessionRequest
    {
        public int StoryId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
    }
}
