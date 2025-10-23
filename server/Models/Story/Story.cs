namespace DragonGame.Models
{
    public class Story
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Act> Acts { get; set; }
    }
}
