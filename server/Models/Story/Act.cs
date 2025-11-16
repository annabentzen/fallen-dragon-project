using System.ComponentModel.DataAnnotations;

namespace DragonGame.Models
{
    public class Act
    {
        [Key]
        public int ActId { get; set; }
        public int ActNumber { get; set; }
        public string Text { get; set; } = string.Empty;

        public int StoryId { get; set; }  // foreign key
        public Story Story { get; set; } = null!;

        public List<Choice> Choices { get; set; } = new();
    
    }
}