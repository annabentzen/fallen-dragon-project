namespace DragonGame.Dtos
{
    public class UpdateCharacterDto
    {
        public int Id { get; set; }  
        public string? Head { get; set; }
        public string? Body { get; set; }
        public int? PoseId { get; set; }
    }

}
