namespace Pinterest111.Models
{
    public class SearchResultsViewModel
    {
        public string Query { get; set; } = "";
        public List<Pin> Pins { get; set; } = new();
        public List<User> Users { get; set; } = new();
    }
}