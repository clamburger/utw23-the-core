namespace UbertweakNfcReaderWeb.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Colour { get; set; }
        public string? Pin { get; set; }
        public bool Admin { get; set; }
    }
}
