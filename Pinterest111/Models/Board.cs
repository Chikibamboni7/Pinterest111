using System;
using System.Collections.Generic;

namespace Pinterest111.Models
{
    public class Board
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Author { get; set; } = ""; // username
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Список id пинов, принадлежащих доске
        public List<int> PinIds { get; set; } = new();
    }
}
