using System.ComponentModel.DataAnnotations;

namespace DragonGame.Models
{
    public class Story
    {
        [Key]
        public int StoryId { get; set; }
        public string Title { get; set; } = null!;
        public List<Act> Acts { get; set; } = null!;
    }
}
