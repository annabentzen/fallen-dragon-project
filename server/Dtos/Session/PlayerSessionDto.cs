using DragonGame.Models;
using System.Text.Json;


namespace DragonGame.Dtos
{
   public class PlayerSessionDto
{
    public int SessionId { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public int CharacterId { get; set; }

    //Design via Character entity
    public string? Hair { get; set; }
    public string? Face { get; set; }
    public string? Outfit { get; set; }
    public int? PoseId { get; set; }

    public int StoryId { get; set; }
    public int CurrentActNumber { get; set; }
    public bool IsCompleted { get; set; }
}

}