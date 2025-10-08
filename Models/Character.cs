using System.Collections.Generic;

namespace MyMvcApp.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Hair { get; set; } = "hair1.png";
        public string Face { get; set; } = "face1.png";
        public string Clothing { get; set; } = "clothing1.png";

        public List<Power> Power { get; set; } = new();
    }
}
