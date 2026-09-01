using System.Collections.Generic;

namespace Pinterest111.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; } = new();
        public List<Pin> Pins { get; set; } = new();
        public bool IsOwnProfile { get; set; }

        // добавлено
        public List<Board> Boards { get; set; } = new();
    }
}
