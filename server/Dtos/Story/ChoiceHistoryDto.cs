
namespace DragonGame.Dtos
{
    public class ChoiceHistoryDto
    {
        public int Id { get; set; }
        public int ActNumber { get; set; }
        public int ChoiceId { get; set; }
        public string ChoiceText { get; set; } = string.Empty;
        public DateTime MadeAt { get; set; }
    }

    public class AddChoiceHistoryRequest
    {
        public int SessionId { get; set; }
        public int ChoiceId { get; set; }
        public int ActNumber { get; set; }
    }
}