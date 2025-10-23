
namespace DragonGame.Models
{
    public class Choice
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int NextActId { get; set; }  // links to next act
    }

}