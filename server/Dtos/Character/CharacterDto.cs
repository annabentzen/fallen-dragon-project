using System.ComponentModel.DataAnnotations;

public class CharacterDto
{
    [Required(ErrorMessage = "Head is required")]
    public string Head { get; set; } = string.Empty;

    [Required(ErrorMessage = "Body is required")]
    public string Body { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Invalid pose ID")]
    public int? PoseId { get; set; }
}