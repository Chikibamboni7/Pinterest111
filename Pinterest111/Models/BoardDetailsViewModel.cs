using System.Collections.Generic;

namespace Pinterest111.Models
{
    public class BoardDetailsViewModel
    {
        public Board Board { get; set; } = new();
        public List<Pin> Pins { get; set; } = new();
        public bool IsOwner { get; set; }
    }
}
