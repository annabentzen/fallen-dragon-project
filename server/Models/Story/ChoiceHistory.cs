using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonGame.Models;

/// <summary>
/// Records each choice a player makes during a session for replay and analytics functionalities
/// </summary>
public class ChoiceHistory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(PlayerSession))]
    public int PlayerSessionId { get; set; }

    public PlayerSession PlayerSession { get; set; } = null!;

    public int ActNumber { get; set; }

    [ForeignKey(nameof(Choice))]
    public int ChoiceId { get; set; }

    public Choice Choice { get; set; } = null!;

    public DateTime MadeAt { get; set; } = DateTime.UtcNow;
}