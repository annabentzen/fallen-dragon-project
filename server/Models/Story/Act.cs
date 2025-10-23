namespace DragonGame.Models
{
    public class Act
    {
        public int Id { get; set; }
        public int StoryId { get; set; }
        public string Text { get; set; } = null!;
        public List<Choice> Choices { get; set; } = new();
        public int? ActNumber { get; set; } 
    }
}