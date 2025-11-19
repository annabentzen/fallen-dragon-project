using System.ComponentModel.DataAnnotations;

namespace DragonGame.Dtos
{
    /// <summary>
    /// Data transfer object for advancing a player session to the next act. 
    /// used when a player makes a chouice that leads to a different act.
    /// </summary>
    public class NextActRequest
    {
        /// <summary>
        /// The next act number to advance to.
        /// if 0 or negative, story has ended
        /// </summary>
        [Required(ErrorMessage = "Next act number is required.")]
        public int NextActNumber { get; set; }
    }
}
