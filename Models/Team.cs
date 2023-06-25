namespace UbertweakNfcReaderWeb.Models
{
    public class Team
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Colour { get; set; }
        public string? Pin { get; set; }
        public int Balance { get; set; } = 0;
    }
}
