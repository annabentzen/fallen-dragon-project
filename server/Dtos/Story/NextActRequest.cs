using System.ComponentModel.DataAnnotations;

namespace DragonGame.Dtos.Story;

public class NextActRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Invalid act number")]
    public int NextActNumber { get; set; }
}