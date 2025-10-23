namespace DragonGame.Models
{
    public class Act
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<Choice> Choices { get; set; }
    }
}