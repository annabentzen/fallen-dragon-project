
namespace DragonGame.Models
{
    public class Choice
    {
         public int ChoiceId { get; set; } // Primary Key
    public string Text { get; set; } = string.Empty; 
    
    public int ActId { get; set; } // Foreign Key to current act
    public Act Act { get; set; } = null!;
    
    // Add this:
    public int? NextActNumber { get; set; } 
    }

}