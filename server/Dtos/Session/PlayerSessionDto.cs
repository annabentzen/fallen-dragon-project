namespace DragonGame.Dtos;

/// <summary>
/// Read-only DTO returned to client
/// </summary>
public class PlayerSessionDto
{
    public int SessionId { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public int CharacterId { get; set; }
    public int UserId { get; set; }
    public string? Head { get; set; }
    public string? Body { get; set; }
    public int? PoseId { get; set; }
    public int StoryId { get; set; }
    public int CurrentActNumber { get; set; }
    public bool IsCompleted { get; set; }
}