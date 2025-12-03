namespace DragonGame.Dtos.Story;

public class ActDto
{
    public int ActNumber { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<ChoiceDto> Choices { get; set; } = new();
    public bool IsEnding { get; set; }
}