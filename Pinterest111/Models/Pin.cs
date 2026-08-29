namespace Pinterest111.Models
{
    public class Pin
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string Author { get; set; } = ""; // username
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
