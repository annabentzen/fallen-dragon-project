using System.Collections.Generic;

namespace MyMvcApp.Models
{
    public class Power
    {
        public int Id { get; set; }
        public string Element { get; set; } = string.Empty;

        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;
    }
}