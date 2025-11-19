using System.ComponentModel.DataAnnotations;

namespace DragonGame.Models
{
    public class Choice
    {
        [Key]
        public int ChoiceId { get; set; } // Primary Key
        public string Text { get; set; } = string.Empty; 
    
        public int ActId { get; set; } // Foreign Key to current act
        public Act Act { get; set; } = null!;
    
        public int NextActNumber { get; set; } 
    }

}