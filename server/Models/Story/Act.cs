using System.ComponentModel.DataAnnotations;

namespace DragonGame.Models
{
    public class Act
    {
        [Key]
        public int ActId { get; set; }
        public int StoryId { get; set; }
        public string Text { get; set; } = null!;
        public List<Choice> Choices { get; set; } = new();
        public int? ActNumber { get; set; } 
    }
}